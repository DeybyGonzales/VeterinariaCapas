# Veterinaria API en capas (S13)

Web API REST con ASP.NET Core (.NET 10), EF Core (base de datos primero), **DTOs** y **AutoMapper**, organizada en cuatro proyectos.
Tema: gestión de los **propietarios** de una veterinaria y sus **mascotas** (relación 1:N).

## Arquitectura
```
Veterinaria.Api        → controladores y Program.cs (solo usa servicios e DTOs)
  └─ Veterinaria.Negocio   → servicios, reglas de negocio, MappingProfile, excepciones
       └─ Veterinaria.Datos     → VeterinariaContext y repositorios
            └─ Veterinaria.Entidades → entidades (Scaffold) y DTOs
```

## Endpoints (11)
| Método | Ruta | Respuestas |
|---|---|---|
| GET | `/api/propietarios?buscar=` | 200 |
| GET | `/api/propietarios/{id}` | 200, 404 |
| GET | `/api/propietarios/{id}/mascotas` (ruta anidada) | 200, 404 |
| POST | `/api/propietarios` | 201, 400, 409 |
| PUT | `/api/propietarios/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/propietarios/{id}` | 204, 404, 409 |
| GET | `/api/mascotas?especie=&propietarioId=&buscar=&edadMax=` | 200 |
| GET | `/api/mascotas/{id}` | 200, 404 |
| POST | `/api/mascotas` | 201, 400, 409 |
| PUT | `/api/mascotas/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/mascotas/{id}` | 204, 404 |

## Reglas de negocio (`ReglaNegocioException` → 409 Conflict)
1. El DNI de un propietario no puede repetirse.
2. No se puede eliminar un propietario que tiene mascotas registradas.
3. Un propietario no puede tener dos mascotas con el mismo nombre.

## Ejecutar en local
```bash
cp .env.example .env          # pon tu clave de SQL Server
docker compose up -d          # SQL Server + database/VeterinariaDB.sql
dotnet user-secrets set "ConnectionStrings:Veterinaria" "Server=localhost,1433;Database=VeterinariaDB;User Id=sa;Password=<tu-clave>;TrustServerCertificate=True;" --project Veterinaria.Api
dotnet run --project Veterinaria.Api --launch-profile http
```
Swagger: http://localhost:5150/swagger · Pruebas: `Veterinaria.Api/Veterinaria.Api.http`

## Scaffold (base de datos primero)
```bash
dotnet ef dbcontext scaffold "Name=ConnectionStrings:Veterinaria" Microsoft.EntityFrameworkCore.SqlServer \
  --project Veterinaria.Datos --startup-project Veterinaria.Api \
  --output-dir Models --context VeterinariaContext --no-onconfiguring --force
```
Después las entidades se movieron a `Veterinaria.Entidades/Entidades` y el contexto a `Veterinaria.Datos/Contexto`.

> La cadena de conexión no está en el repositorio (se usa `dotnet user-secrets`).
