using System.Security.Cryptography;
using System.Text;
using LibraFinanzas.Models;

namespace LibraFinanzas.Services;

/// <summary>
/// Mantiene el estado del usuario que inició sesión en el circuito actual de Blazor Server.
/// </summary>
public class SesionService
{
    public Usuario? UsuarioActual { get; private set; }

    public bool EstaAutenticado => UsuarioActual is not null;

    public int IdUsuarioActual => UsuarioActual?.IdUsuario ?? 0;

    /// <summary>Evento que avisa al layout y menú lateral cuando alguien inicia, edita o cierra sesión.</summary>
    public event Action? OnCambioSesion;

    public void IniciarSesion(Usuario usuario)
    {
        UsuarioActual = usuario;
        OnCambioSesion?.Invoke();
    }

    public void ActualizarUsuario(Usuario usuario)
    {
        UsuarioActual = usuario;
        OnCambioSesion?.Invoke();
    }

    public void CerrarSesion()
    {
        UsuarioActual = null;
        OnCambioSesion?.Invoke();
    }

    /// <summary>Convierte una contraseña a su hash SHA-256 para no guardarla en texto plano.</summary>
    public static string GenerarHash(string texto)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(texto);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
