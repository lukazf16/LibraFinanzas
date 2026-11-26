using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraFinanzas.Models;

/// <summary>
/// Representa un ingreso o gasto registrado por el usuario.
/// </summary>
[Table("Movimiento")]
public class Movimiento
{
    [Key]
    [Column("id_movimiento")]
    public int IdMovimiento { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("tipo")]
    [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
    [StringLength(20)]
    public string Tipo { get; set; } = "Gasto"; // "Ingreso" o "Gasto"

    [Column("descripcion")]
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(120, ErrorMessage = "La descripción no debe exceder los 120 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Column("monto", TypeName = "decimal(12,2)")]
    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, 999999999, ErrorMessage = "El monto debe ser mayor a $0.00.")]
    public decimal Monto { get; set; }

    [Column("categoria")]
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(40)]
    public string Categoria { get; set; } = "Comida";

    [Column("clasificacion")]
    [Required(ErrorMessage = "La clasificación es obligatoria.")]
    [StringLength(30)]
    public string Clasificacion { get; set; } = "Necesario"; // "Ingreso", "Necesario" o "Innecesario"

    [Column("fecha")]
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    // Relación con Usuario
    public Usuario? Usuario { get; set; }
}
