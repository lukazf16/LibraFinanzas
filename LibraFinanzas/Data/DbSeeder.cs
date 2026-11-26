using LibraFinanzas.Models;
using LibraFinanzas.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraFinanzas.Data;

/// <summary>
/// Crea la base de datos automáticamente si no existe y carga a la usuaria inicial Mariana Guzmán.
/// </summary>
public static class DbSeeder
{
    public const string CorreoDemo = "mariana.guzman@correo.com";
    public const string ContrasenaDemo = "Libra2026!";

    public static async Task InicializarAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        // Crea la base de datos y las tablas si todavía no existen en SQL Server
        await db.Database.EnsureCreatedAsync();

        // Si existía una versión previa de la BD sin la columna 'contrasena', la regenera limpiamente
        try
        {
            _ = await db.Usuarios.Select(u => u.Contrasena).FirstOrDefaultAsync();
        }
        catch
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        // Si ya existen usuarios registrados, no se vuelve a sembrar
        if (await db.Usuarios.AnyAsync())
        {
            return;
        }

        // Usuaria inicial registrada basada en el diseño original de Libra
        var mariana = new Usuario
        {
            Nombre = "Mariana Guzmán",
            Correo = CorreoDemo,
            Telefono = "+52 312 145 8890",
            Contrasena = SesionService.GenerarHash(ContrasenaDemo),
            SaldoInicial = 38825.50m,
            Activo = true
        };

        db.Usuarios.Add(mariana);
        await db.SaveChangesAsync();

        // Fechas dentro del mes actual para que los totales del mes reflejen los datos reales
        DateTime hoy = DateTime.Today;
        DateTime DiaDelMes(int diasAtras)
        {
            DateTime fecha = hoy.AddDays(-diasAtras);
            if (fecha.Month != hoy.Month)
            {
                return new DateTime(hoy.Year, hoy.Month, 1);
            }
            return fecha;
        }

        var movimientosIniciales = new List<Movimiento>
        {
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoIngreso,
                Descripcion = "Pago de nómina",
                Monto = 10750.00m,
                Categoria = "Salario",
                Clasificacion = CatalogosFinancieros.ClasificacionIngreso,
                Fecha = DiaDelMes(0)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Café para llevar",
                Monto = 68.00m,
                Categoria = "Comida",
                Clasificacion = CatalogosFinancieros.ClasificacionInnecesario,
                Fecha = DiaDelMes(0)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Renta del departamento",
                Monto = 6200.00m,
                Categoria = "Casa",
                Clasificacion = CatalogosFinancieros.ClasificacionNecesario,
                Fecha = DiaDelMes(2)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Pedido a domicilio",
                Monto = 340.00m,
                Categoria = "Comida",
                Clasificacion = CatalogosFinancieros.ClasificacionInnecesario,
                Fecha = DiaDelMes(3)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Blusa en oferta",
                Monto = 499.00m,
                Categoria = "Ropa",
                Clasificacion = CatalogosFinancieros.ClasificacionInnecesario,
                Fecha = DiaDelMes(5)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Salida al cine y snacks",
                Monto = 620.00m,
                Categoria = "Entretenimiento",
                Clasificacion = CatalogosFinancieros.ClasificacionInnecesario,
                Fecha = DiaDelMes(6)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Servicios de luz, agua e internet",
                Monto = 1860.00m,
                Categoria = "Casa",
                Clasificacion = CatalogosFinancieros.ClasificacionNecesario,
                Fecha = DiaDelMes(7)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Transporte y despensa básica",
                Monto = 1800.00m,
                Categoria = "Transporte",
                Clasificacion = CatalogosFinancieros.ClasificacionNecesario,
                Fecha = DiaDelMes(8)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoIngreso,
                Descripcion = "Diseño web freelance",
                Monto = 10750.00m,
                Categoria = "Freelance",
                Clasificacion = CatalogosFinancieros.ClasificacionIngreso,
                Fecha = DiaDelMes(9)
            },
            new Movimiento
            {
                IdUsuario = mariana.IdUsuario,
                Tipo = CatalogosFinancieros.TipoGasto,
                Descripcion = "Suscripciones de streaming",
                Monto = 618.00m,
                Categoria = "Suscripciones",
                Clasificacion = CatalogosFinancieros.ClasificacionInnecesario,
                Fecha = DiaDelMes(10)
            }
        };

        db.Movimientos.AddRange(movimientosIniciales);

        var metasIniciales = new List<MetaAhorro>
        {
            new MetaAhorro
            {
                IdUsuario = mariana.IdUsuario,
                Nombre = "Fondo de emergencia",
                MontoObjetivo = 30000.00m,
                MontoActual = 18400.00m,
                FechaLimite = new DateTime(hoy.Year, 12, 31),
                ColorBarra = "emerald"
            },
            new MetaAhorro
            {
                IdUsuario = mariana.IdUsuario,
                Nombre = "Viaje de fin de año",
                MontoObjetivo = 15000.00m,
                MontoActual = 6200.00m,
                FechaLimite = new DateTime(hoy.Year, 12, 20),
                ColorBarra = "gold"
            },
            new MetaAhorro
            {
                IdUsuario = mariana.IdUsuario,
                Nombre = "Laptop nueva",
                MontoObjetivo = 13000.00m,
                MontoActual = 9750.00m,
                FechaLimite = new DateTime(hoy.Year, 12, 15),
                ColorBarra = "navy"
            }
        };

        db.MetasAhorro.AddRange(metasIniciales);
        await db.SaveChangesAsync();
    }
}
