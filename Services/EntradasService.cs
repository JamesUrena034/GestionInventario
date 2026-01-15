using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using GestionEntradasInventario.Data;
using GestionEntradasInventario.Models;

namespace GestionEntradasInventario.Services;

public class EntradasService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Existe(int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Entradas.AnyAsync(e => e.EntradaId == entradaId);
    }

    public async Task<bool> Insertar(Entradas entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        entrada.Total = entrada.EntradasDetalle.Sum(d => d.Costo * d.Cantidad);
        contexto.Entradas.Add(entrada);
        await AfectarInventario(entrada.EntradasDetalle.ToArray(), TipoOperacion.Suma);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task AfectarInventario(EntradasDetalle[] detalles, TipoOperacion tipoOperacion)
    {
        if (detalles == null || detalles.Length == 0)
            return;

        await using var contexto = await DbFactory.CreateDbContextAsync();

        foreach (var item in detalles)
        {
            var producto = await contexto.Productos.SingleAsync(p => p.ProductoId == item.ProductoId);

            if (tipoOperacion == TipoOperacion.Suma)
                producto.Existencia += item.Cantidad;
            else if (tipoOperacion == TipoOperacion.Resta)
                producto.Existencia -= item.Cantidad;
        }

        await contexto.SaveChangesAsync();
    }

    public async Task<bool> Modificar(Entradas entrada)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entradaAnterior = await contexto.Entradas
            .Include(e => e.EntradasDetalle)
            .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EntradaId == entrada.EntradaId);

        if (entradaAnterior == null)
            return false;

        await AfectarInventario(entradaAnterior.EntradasDetalle.ToArray(), TipoOperacion.Resta);

        entrada.Total = entrada.EntradasDetalle.Sum(d => d.Costo * d.Cantidad);
        await AfectarInventario(entrada.EntradasDetalle.ToArray(), TipoOperacion.Suma);

        contexto.Entradas.Update(entrada);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Entradas entrada)
    {
        if (!await Existe(entrada.EntradaId))
            return await Insertar(entrada);
        else
            return await Modificar(entrada);
    }

    public async Task<Entradas?> Buscar(int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        return await contexto.Entradas
            .Include(e => e.EntradasDetalle)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(e => e.EntradaId == entradaId);
    }

    public async Task<bool> Eliminar(int entradaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var entrada = await contexto.Entradas
            .Include(e => e.EntradasDetalle)
            .FirstOrDefaultAsync(e => e.EntradaId == entradaId);

        if (entrada == null)
            return false;
        
        await AfectarInventario(entrada.EntradasDetalle.ToArray(), TipoOperacion.Resta);

        contexto.EntradasDetalle.RemoveRange(entrada.EntradasDetalle);
        contexto.Entradas.Remove(entrada);

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<List<Entradas>> GetList(Expression<Func<Entradas, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        return await contexto.Entradas
            .Include(e => e.EntradasDetalle)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public enum TipoOperacion
    {
        Suma = 1,
        Resta = 2
    }

    public async Task<List<Producto>> ListarProductos()
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos.AsNoTracking().ToListAsync();
    }
}