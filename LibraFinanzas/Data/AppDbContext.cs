using LibraFinanzas.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraFinanzas.Data;

/// <summary>
/// Contexto de Entity Framework Core que enlaza las clases C# con las tablas de SQL Server.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<MetaAhorro> MetasAhorro => Set<MetaAhorro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // El correo de cada usuario debe ser único para poder iniciar sesión sin duplicados
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        // Un usuario tiene muchos movimientos
        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.Movimientos)
            .HasForeignKey(m => m.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // Un usuario tiene muchas metas de ahorro
        modelBuilder.Entity<MetaAhorro>()
            .HasOne(meta => meta.Usuario)
            .WithMany(u => u.Metas)
            .HasForeignKey(meta => meta.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
