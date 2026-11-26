using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraFinanzas.Models;

/// <summary>
/// Representa una meta de ahorro personal con seguimiento de avance.
/// </summary>
[Table("MetaAhorro")]
public class MetaAhorro
{
    [Key]
    [Column("id_meta")]
    public int IdMeta { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("nombre")]
    [Required(ErrorMessage = "El nombre de la meta es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Column("monto_objetivo", TypeName = "decimal(12,2)")]
    [Range(1, 999999999, ErrorMessage = "El monto objetivo debe ser mayor a $0.00.")]
    public decimal MontoObjetivo { get; set; }

    [Column("monto_actual", TypeName = "decimal(12,2)")]
    [Range(0, 999999999, ErrorMessage = "El monto ahorrado no puede ser negativo.")]
    public decimal MontoActual { get; set; }

    [Column("fecha_limite")]
    [Required(ErrorMessage = "La fecha límite es obligatoria.")]
    public DateTime FechaLimite { get; set; } = DateTime.Today.AddMonths(3);

    [Column("color_barra")]
    [StringLength(20)]
    public string ColorBarra { get; set; } = "emerald"; // "emerald", "gold", "navy"

    // Relación con Usuario
    public Usuario? Usuario { get; set; }

    /// <summary>
    /// Calcula el porcentaje de avance entre 0 y 100.
    /// </summary>
    public int CalcularPorcentaje()
    {
        if (MontoObjetivo <= 0)
        {
            return 0;
        }

        decimal porcentaje = (MontoActual / MontoObjetivo) * 100m;
        int redondeado = Convert.ToInt32(Math.Round(porcentaje));
        return redondeado > 100 ? 100 : redondeado;
    }

    /// <summary>
    /// Calcula cuánto dinero falta para completar la meta.
    /// </summary>
    public decimal CalcularFaltante()
    {
        decimal restante = MontoObjetivo - MontoActual;
        return restante > 0 ? restante : 0;
    }
}
