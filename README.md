# Libra — Panel Financiero Personal (Blazor + SQL Server)

Aplicación web desarrollada con **Blazor Server (.NET 10)**, **Entity Framework Core** y **SQL Server LocalDB** para la materia de **Programación Web**, con **inicio de sesión, registro de cuentas nuevas (en blanco) y datos privados por usuario**.

## Credenciales de la usuaria registrada (datos de ejemplo)

- **Correo:** `mariana.guzman@correo.com`
- **Contraseña:** `Libra2026!`

Si creas una cuenta nueva desde la pestaña **"Crear cuenta nueva"**, entrarás con el panel **completamente en blanco** (`$0.00` en todas las métricas, sin movimientos ni metas) para capturar tus propios registros.

## Requisitos

- **Visual Studio 2022 / 2026** con la carga de trabajo *Desarrollo de ASP.NET y web* (incluye .NET 10 SDK y SQL Server LocalDB).
- O bien **.NET 10 SDK** desde línea de comandos.

## Cómo abrir y ejecutar en Visual Studio

1. Abrir el archivo `LibraFinanzas.sln` en Visual Studio.
2. Presionar **F5** (o el botón de ejecución **https**).
3. Al iniciar por primera vez, la aplicación crea automáticamente la base de datos `LibraFinanzas` en `(localdb)\MSSQLLocalDB` y carga a la usuaria inicial mediante `Data/DbSeeder.cs`.

## Estructura del proyecto

| Carpeta / Archivo | Propósito |
|---|---|
| `Models/` | Clases de negocio (`Usuario`, `Movimiento`, `MetaAhorro`, `AccesoModelos`, `CatalogosFinancieros`) con validaciones `DataAnnotations`. |
| `Data/` | `AppDbContext` (enlace con SQL Server) y `DbSeeder` (creación automática de tablas y datos de Mariana). |
| `Services/SesionService.cs` | Estado de la sesión activa del usuario y hash SHA-256 de contraseñas. |
| `Services/FinanzasService.cs` | Inicio de sesión, registro en blanco y consultas CRUD filtradas por `IdUsuario`. |
| `Components/Layout/` | `MainLayout.razor` (protege las vistas si no hay sesión) y `NavMenu.razor` (barra lateral con perfil y botón de cerrar sesión). |
| `Components/Shared/` | `PantallaAcceso.razor` (login y registro), `TarjetaMetrica.razor` y `PanelContenido.razor`. |
| `Components/Pages/` | Páginas interactivas: `Home`, `Movimientos`, `Metas`, `Reportes` y `Ajustes`. |
| `wwwroot/app.css` | Hoja de estilos editorial financiera de Libra. |
