using System.ComponentModel.DataAnnotations;

namespace GestionEntradasInventario.Models;

public class Producto
{
    [Key]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "Debe tener una descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
    public double Costo { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public double Precio { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La existencia no puede ser negativa")]
    public double Existencia { get; set; }
}