using Microsoft.EntityFrameworkCore;
using GestionEntradasInventario.Data;
using GestionEntradasInventario.Models;
using System.Linq.Expressions;

namespace GestionEntradasInventario.Services;

public class ProductosService(IDbContextFactory<ApplicationDbContext> DbFactory)
{
    public async Task<bool> Existe(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos.AnyAsync(p => p.ProductoId == productoId);
    }

    private async Task<bool> Insertar(Producto producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Productos.Add(producto);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Producto producto)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Productos.Update(producto);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Producto producto)
    {
        if (!await Existe(producto.ProductoId))
            return await Insertar(producto);
        else
            return await Modificar(producto);
    }

    public async Task<Producto?> Buscar(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductoId == productoId);
    }

    public async Task<bool> Eliminar(int productoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .Where(p => p.ProductoId == productoId)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<List<Producto>> GetList(Expression<Func<Producto, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Productos
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}