using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionEntradasInventario.Models;

public class EntradasDetalle
{
    [Key]
    public int DetalleId { get; set; }

    public int EntradaId { get; set; }

    public int ProductoId { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public double Cantidad { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
    public double Costo { get; set; }

    [ForeignKey("EntradaId")]
    [InverseProperty("EntradasDetalle")]
    public virtual Entradas Entrada { get; set; } = null!;

    [ForeignKey("ProductoId")]
    public virtual Producto Producto { get; set; } = null!;
}