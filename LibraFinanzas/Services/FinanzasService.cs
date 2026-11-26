using LibraFinanzas.Data;
using LibraFinanzas.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraFinanzas.Services;

/// <summary>
/// Objeto auxiliar para transportar los totales calculados del panel principal.
/// </summary>
public class ResumenFinanciero
{
    public decimal PatrimonioNeto { get; set; }
    public decimal IngresosMes { get; set; }
    public decimal GastosNecesariosMes { get; set; }
    public decimal GastosHormigaMes { get; set; }
    public decimal PorcentajeHormiga { get; set; }
    public int CantidadMovimientos { get; set; }
    public decimal BalanceMes => IngresosMes - (GastosNecesariosMes + GastosHormigaMes);
    public decimal AhorroSugeridoHormiga => Math.Round(GastosHormigaMes / 2m, 2);
}

/// <summary>
/// Servicio de lógica de negocio (capa Back-End) para autenticación y operaciones CRUD por usuario en SQL Server.
/// </summary>
public class FinanzasService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public FinanzasService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    // ==================== AUTENTICACIÓN Y REGISTRO ====================

    /// <summary>Valida el correo y contraseña contra la tabla Usuario en SQL Server.</summary>
    public async Task<(Usuario? Usuario, string? Error)> IniciarSesionAsync(string correo, string contrasena)
    {
        string correoNormalizado = correo.Trim().ToLower();
        string hashIngresado = SesionService.GenerarHash(contrasena);

        await using var db = await _factory.CreateDbContextAsync();
        var usuario = await db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Correo.ToLower() == correoNormalizado);

        if (usuario is null || usuario.Contrasena != hashIngresado)
        {
            return (null, "Correo o contraseña incorrectos. Verifica tus datos e intenta de nuevo.");
        }

        if (!usuario.Activo)
        {
            return (null, "Esta cuenta se encuentra inactiva.");
        }

        return (usuario, null);
    }

    /// <summary>Registra un nuevo usuario en SQL Server completamente en blanco (saldo $0, sin movimientos ni metas).</summary>
    public async Task<(Usuario? Usuario, string? Error)> RegistrarUsuarioAsync(RegistroModelo datos)
    {
        string correoNormalizado = datos.Correo.Trim().ToLower();

        await using var db = await _factory.CreateDbContextAsync();
        bool correoExiste = await db.Usuarios.AnyAsync(u => u.Correo.ToLower() == correoNormalizado);
        if (correoExiste)
        {
            return (null, "Ya existe una cuenta registrada con ese correo electrónico.");
        }

        var nuevoUsuario = new Usuario
        {
            Nombre = datos.Nombre.Trim(),
            Correo = correoNormalizado,
            Telefono = datos.Telefono.Trim(),
            Contrasena = SesionService.GenerarHash(datos.Contrasena),
            SaldoInicial = 0m,
            Activo = true
        };

        db.Usuarios.Add(nuevoUsuario);
        await db.SaveChangesAsync();

        return (nuevoUsuario, null);
    }

    // ==================== PERFIL DE USUARIO ====================

    /// <summary>Obtiene el perfil del usuario por su Id.</summary>
    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
    }

    /// <summary>Actualiza los datos del usuario en SQL Server.</summary>
    public async Task<(Usuario? UsuarioActualizado, string? Error)> GuardarUsuarioAsync(Usuario usuario)
    {
        string correoNormalizado = usuario.Correo.Trim().ToLower();

        await using var db = await _factory.CreateDbContextAsync();

        bool correoDuplicado = await db.Usuarios.AnyAsync(u =>
            u.IdUsuario != usuario.IdUsuario && u.Correo.ToLower() == correoNormalizado);
        if (correoDuplicado)
        {
            return (null, "Ese correo electrónico ya está en uso por otra cuenta.");
        }

        var actual = await db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
        if (actual is null)
        {
            return (null, "No se encontró el usuario en la base de datos.");
        }

        actual.Nombre = usuario.Nombre.Trim();
        actual.Correo = correoNormalizado;
        actual.Telefono = usuario.Telefono.Trim();
        actual.SaldoInicial = usuario.SaldoInicial;
        actual.Activo = usuario.Activo;
        await db.SaveChangesAsync();

        return (actual, null);
    }

    // ==================== RESUMEN Y MOVIMIENTOS (POR USUARIO) ====================

    /// <summary>Calcula las métricas financieras del mes actual y el patrimonio neto del usuario conectado.</summary>
    public async Task<ResumenFinanciero> ObtenerResumenAsync(int idUsuario)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
        var todos = await db.Movimientos
            .AsNoTracking()
            .Where(m => m.IdUsuario == idUsuario)
            .ToListAsync();

        decimal totalIngresosHistorico = 0m;
        decimal totalGastosHistorico = 0m;
        decimal ingresosMes = 0m;
        decimal gastosNecesariosMes = 0m;
        decimal gastosHormigaMes = 0m;

        DateTime hoy = DateTime.Today;

        foreach (Movimiento m in todos)
        {
            bool esDelMesActual = m.Fecha.Year == hoy.Year && m.Fecha.Month == hoy.Month;

            if (m.Tipo == CatalogosFinancieros.TipoIngreso)
            {
                totalIngresosHistorico += m.Monto;
                if (esDelMesActual)
                {
                    ingresosMes += m.Monto;
                }
            }
            else
            {
                totalGastosHistorico += m.Monto;
                if (esDelMesActual)
                {
                    if (m.Clasificacion == CatalogosFinancieros.ClasificacionInnecesario)
                    {
                        gastosHormigaMes += m.Monto;
                    }
                    else
                    {
                        gastosNecesariosMes += m.Monto;
                    }
                }
            }
        }

        decimal saldoInicial = usuario != null ? usuario.SaldoInicial : 0m;
        decimal patrimonio = saldoInicial + totalIngresosHistorico - totalGastosHistorico;
        decimal porcentajeHormiga = ingresosMes > 0
            ? Math.Round((gastosHormigaMes / ingresosMes) * 100m, 1)
            : 0m;

        return new ResumenFinanciero
        {
            PatrimonioNeto = patrimonio,
            IngresosMes = ingresosMes,
            GastosNecesariosMes = gastosNecesariosMes,
            GastosHormigaMes = gastosHormigaMes,
            PorcentajeHormiga = porcentajeHormiga,
            CantidadMovimientos = todos.Count
        };
    }

    /// <summary>Obtiene los últimos movimientos del usuario conectado.</summary>
    public async Task<List<Movimiento>> ObtenerMovimientosRecientesAsync(int idUsuario, int cantidad)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Movimientos
            .AsNoTracking()
            .Where(m => m.IdUsuario == idUsuario)
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.IdMovimiento)
            .Take(cantidad)
            .ToListAsync();
    }

    /// <summary>Obtiene los movimientos del usuario conectado aplicando filtros opcionales.</summary>
    public async Task<List<Movimiento>> ObtenerMovimientosAsync(
        int idUsuario,
        string? tipo = null,
        string? clasificacion = null,
        string? categoria = null,
        DateTime? desde = null,
        DateTime? hasta = null)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var consulta = db.Movimientos
            .AsNoTracking()
            .Where(m => m.IdUsuario == idUsuario)
            .AsQueryable();

        if (!string.IsNullOrEmpty(tipo) && tipo != "Todos")
        {
            consulta = consulta.Where(m => m.Tipo == tipo);
        }

        if (!string.IsNullOrEmpty(clasificacion) && clasificacion != "Todas")
        {
            consulta = consulta.Where(m => m.Clasificacion == clasificacion);
        }

        if (!string.IsNullOrEmpty(categoria) && categoria != "Todas")
        {
            consulta = consulta.Where(m => m.Categoria == categoria);
        }

        if (desde.HasValue)
        {
            DateTime inicio = desde.Value.Date;
            consulta = consulta.Where(m => m.Fecha >= inicio);
        }

        if (hasta.HasValue)
        {
            DateTime fin = hasta.Value.Date.AddDays(1);
            consulta = consulta.Where(m => m.Fecha < fin);
        }

        return await consulta
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.IdMovimiento)
            .ToListAsync();
    }

    /// <summary>Inserta o actualiza un movimiento perteneciente al usuario conectado.</summary>
    public async Task GuardarMovimientoAsync(int idUsuario, Movimiento movimiento)
    {
        await using var db = await _factory.CreateDbContextAsync();

        if (movimiento.Tipo == CatalogosFinancieros.TipoIngreso)
        {
            movimiento.Clasificacion = CatalogosFinancieros.ClasificacionIngreso;
        }
        else if (movimiento.Clasificacion == CatalogosFinancieros.ClasificacionIngreso)
        {
            movimiento.Clasificacion = CatalogosFinancieros.ClasificacionNecesario;
        }

        if (movimiento.IdMovimiento == 0)
        {
            movimiento.IdUsuario = idUsuario;
            movimiento.Descripcion = movimiento.Descripcion.Trim();
            db.Movimientos.Add(movimiento);
        }
        else
        {
            var actual = await db.Movimientos.FirstOrDefaultAsync(m =>
                m.IdMovimiento == movimiento.IdMovimiento && m.IdUsuario == idUsuario);
            if (actual != null)
            {
                actual.Tipo = movimiento.Tipo;
                actual.Descripcion = movimiento.Descripcion.Trim();
                actual.Monto = movimiento.Monto;
                actual.Categoria = movimiento.Categoria;
                actual.Clasificacion = movimiento.Clasificacion;
                actual.Fecha = movimiento.Fecha;
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Elimina un movimiento perteneciente al usuario conectado.</summary>
    public async Task EliminarMovimientoAsync(int idUsuario, int idMovimiento)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var encontrado = await db.Movimientos.FirstOrDefaultAsync(m =>
            m.IdMovimiento == idMovimiento && m.IdUsuario == idUsuario);
        if (encontrado != null)
        {
            db.Movimientos.Remove(encontrado);
            await db.SaveChangesAsync();
        }
    }

    // ==================== METAS DE AHORRO (POR USUARIO) ====================

    /// <summary>Obtiene las metas de ahorro del usuario conectado.</summary>
    public async Task<List<MetaAhorro>> ObtenerMetasAsync(int idUsuario)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.MetasAhorro
            .AsNoTracking()
            .Where(m => m.IdUsuario == idUsuario)
            .OrderBy(m => m.FechaLimite)
            .ToListAsync();
    }

    /// <summary>Inserta o actualiza una meta de ahorro del usuario conectado.</summary>
    public async Task GuardarMetaAsync(int idUsuario, MetaAhorro meta)
    {
        await using var db = await _factory.CreateDbContextAsync();

        if (meta.IdMeta == 0)
        {
            meta.IdUsuario = idUsuario;
            meta.Nombre = meta.Nombre.Trim();
            db.MetasAhorro.Add(meta);
        }
        else
        {
            var actual = await db.MetasAhorro.FirstOrDefaultAsync(m =>
                m.IdMeta == meta.IdMeta && m.IdUsuario == idUsuario);
            if (actual != null)
            {
                actual.Nombre = meta.Nombre.Trim();
                actual.MontoObjetivo = meta.MontoObjetivo;
                actual.MontoActual = meta.MontoActual;
                actual.FechaLimite = meta.FechaLimite;
                actual.ColorBarra = meta.ColorBarra;
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>Suma un abono a una meta de ahorro del usuario conectado.</summary>
    public async Task<string?> AbonarMetaAsync(int idUsuario, int idMeta, decimal abono)
    {
        if (abono <= 0)
        {
            return "El monto del abono debe ser mayor a $0.00.";
        }

        await using var db = await _factory.CreateDbContextAsync();
        var meta = await db.MetasAhorro.FirstOrDefaultAsync(m =>
            m.IdMeta == idMeta && m.IdUsuario == idUsuario);
        if (meta == null)
        {
            return "La meta seleccionada no existe.";
        }

        meta.MontoActual += abono;
        await db.SaveChangesAsync();
        return null;
    }

    /// <summary>Elimina una meta de ahorro del usuario conectado.</summary>
    public async Task EliminarMetaAsync(int idUsuario, int idMeta)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var meta = await db.MetasAhorro.FirstOrDefaultAsync(m =>
            m.IdMeta == idMeta && m.IdUsuario == idUsuario);
        if (meta != null)
        {
            db.MetasAhorro.Remove(meta);
            await db.SaveChangesAsync();
        }
    }
}
