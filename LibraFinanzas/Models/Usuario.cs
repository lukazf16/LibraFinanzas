using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraFinanzas.Models;

/// <summary>
/// Representa al usuario dueño de una cuenta en el panel financiero personal.
/// Incluye credenciales para iniciar sesión y validaciones de DataAnnotations.
/// </summary>
[Table("Usuario")]
public class Usuario
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Column("correo")]
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El correo no debe exceder los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "Debe escribir un correo electrónico válido.")]
    public string Correo { get; set; } = string.Empty;

    [Column("telefono")]
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20, ErrorMessage = "El teléfono no debe exceder los 20 caracteres.")]
    [Phone(ErrorMessage = "Debe escribir un número de teléfono válido.")]
    public string Telefono { get; set; } = string.Empty;

    [Column("contrasena")]
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100)]
    public string Contrasena { get; set; } = string.Empty;

    [Column("saldo_inicial", TypeName = "decimal(12,2)")]
    [Range(0, 999999999, ErrorMessage = "El saldo inicial debe ser mayor o igual a 0.")]
    public decimal SaldoInicial { get; set; }

    [Column("activo")]
    public bool Activo { get; set; } = true;

    // Propiedades de navegación (relación 1 a muchos)
    public List<Movimiento> Movimientos { get; set; } = new();
    public List<MetaAhorro> Metas { get; set; } = new();

    /// <summary>
    /// Obtiene las iniciales del nombre para mostrarlas en el avatar.
    /// </summary>
    public string ObtenerIniciales()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            return "US";
        }

        string[] partes = Nombre.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 1)
        {
            return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        }

        return (partes[0][0].ToString() + partes[1][0].ToString()).ToUpper();
    }

    /// <summary>
    /// Obtiene solo el primer nombre para el saludo del panel principal.
    /// </summary>
    public string ObtenerPrimerNombre()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            return "Usuario";
        }

        string[] partes = Nombre.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes[0];
    }
}
