# Portal Académico - Examen Parcial

Sistema de gestión de cursos y matrículas desarrollado en ASP.NET Core MVC (.NET 8).

## Pasos para ejecución local

1. Clonar el repositorio.
2. Instalar el SDK de .NET 8.
3. Configurar la cadena de conexión en `appsettings.json`.
4. Ejecutar las migraciones en la terminal: 
   ```bash
   dotnet ef database update
5. Ejectuar el proyecto:
   dotnet run

## Variables de entorno necesarias (Producción)
- ASPNETCORE_ENVIRONMENT: Production
- ASPNETCORE_URLS: http://0.0.0.0:${PORT}
- ConnectionStrings__DefaultConnection: (Cadena de conexión a base de datos)
- REDIS_CONNECTION_STRING: (Credenciales y URL de servidor Redis en la nube))

## URL del despliegue
El proyecto se encuentra desplegado y funcionando en Render:
https://portalacademico-5f0k.onrender.com
