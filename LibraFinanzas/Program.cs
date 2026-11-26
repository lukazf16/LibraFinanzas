using LibraFinanzas.Components;
using LibraFinanzas.Data;
using LibraFinanzas.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agrega soporte para componentes Razor con renderizado interactivo en el servidor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra la fábrica de contextos de Entity Framework Core apuntando a SQL Server (LocalDB)
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra los servicios de sesión y lógica financiera para inyectarlos en las páginas Blazor
builder.Services.AddScoped<SesionService>();
builder.Services.AddScoped<FinanzasService>();

var app = builder.Build();

// Crea la base de datos (si no existe) y carga los datos iniciales de Mariana Guzmán
await DbSeeder.InicializarAsync(app.Services);

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
