# Portal Académico - Examen Parcial

Sistema de gestión de cursos y matrículas desarrollado en ASP.NET Core MVC (.NET 8).

## Pasos para ejecución local
1. Clonar el repositorio.
2. Instalar el SDK de .NET 8.
3. Configurar la cadena de conexión en `appsettings.json`.
4. Ejecutar las migraciones: `dotnet ef database update`.
5. Ejecutar: `dotnet run`.

## Variables de entorno necesarias
* `ASPNETCORE_ENVIRONMENT`: Production
* `ConnectionStrings__DefaultConnection`: (Tu cadena de conexión a SQLite)
* `Redis__ConnectionString`: (URL de tu servidor Redis en la nube)

## URL del despliegue
https://portalacademico-5f0k.onrender.com
