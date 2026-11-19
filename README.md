# Veterinaria API en capas con JWT (S13 + S14)

Web API REST con ASP.NET Core (.NET 10), EF Core (base de datos primero), **DTOs**, **AutoMapper** y **autenticación JWT con roles**, organizada en cuatro proyectos.
Tema: gestión de los **propietarios** de una veterinaria y sus **mascotas** (relación 1:N), con acceso según el puesto de cada usuario.

## Arquitectura
```
Veterinaria.Api        → controladores (Auth, Propietarios, Mascotas) y Program.cs (JWT, Swagger)
  └─ Veterinaria.Negocio   → servicios (incl. AuthServicio: BCrypt + token), reglas, MappingProfile, Roles
       └─ Veterinaria.Datos     → VeterinariaContext y repositorios (incl. UsuarioRepositorio)
            └─ Veterinaria.Entidades → entidades (Propietario, Mascota, Usuario) y DTOs
```

## Roles (S14)
| Rol | Quién es | Qué puede hacer |
|---|---|---|
| **Cliente** | Dueño de mascota que se registra por su cuenta (rol por defecto) | Consultar propietarios y mascotas, ver su perfil |
| **Recepcionista** | Personal de mostrador | Registrar, editar y eliminar propietarios; registrar y dar de baja mascotas |
| **Veterinario** | Médico que atiende las consultas | Actualizar los datos clínicos de una mascota (peso, edad) |

El rol **lo asigna el servidor**: todo registro nace como `Cliente` (aunque el cliente envíe `"rol"`), y la clínica asciende a Recepcionista o Veterinario con un `UPDATE`.

## Matriz de permisos
| Endpoint | Sin token | Cliente | Recepcionista | Veterinario |
|---|---|---|---|---|
| POST `/api/auth/registro` | Sí | Sí | Sí | Sí |
| POST `/api/auth/login` | Sí | Sí | Sí | Sí |
| GET `/api/auth/perfil` | No (401) | Sí | Sí | Sí |
| GET `/api/mascotas`, `/api/mascotas/{id}` | Sí | Sí | Sí | Sí |
| POST `/api/mascotas` | No (401) | No (403) | Sí | No (403) |
| PUT `/api/mascotas/{id}` | No (401) | No (403) | No (403) | Sí |
| DELETE `/api/mascotas/{id}` | No (401) | No (403) | Sí | No (403) |
| GET `/api/propietarios`, `/{id}`, `/{id}/mascotas` | No (401) | Sí | Sí | Sí |
| POST `/api/propietarios` | No (401) | No (403) | Sí | No (403) |
| PUT `/api/propietarios/{id}` | No (401) | No (403) | Sí | No (403) |
| DELETE `/api/propietarios/{id}` | No (401) | No (403) | Sí | No (403) |

Público: registro, login y consulta de mascotas · Solo autenticado: perfil y consulta de propietarios · Rol específico: escrituras.

## Token JWT
- Claims: `sub` (Id del usuario), `email`, `role`, `jti`, más `iss`, `aud` y `exp`.
- Se valida emisor, audiencia, vencimiento (sin tolerancia) y firma HMAC-SHA256.
- `Jwt:Emisor`, `Jwt:Audiencia` y `Jwt:MinutosExpiracion` están en `appsettings.json`; **`Jwt:Clave` (≥ 32 caracteres) no está en el repositorio**: se guarda con `dotnet user-secrets`.

## Reglas de negocio (`ReglaNegocioException` → 409 Conflict)
1. El DNI de un propietario no puede repetirse.
2. No se puede eliminar un propietario que tiene mascotas registradas.
3. Un propietario no puede tener dos mascotas con el mismo nombre.
4. El correo de un usuario es único.

## Ejecutar en local
```bash
cp .env.example .env          # pon tu clave de SQL Server
docker compose up -d          # SQL Server + database/VeterinariaDB.sql (incluye Usuarios)
dotnet user-secrets set "ConnectionStrings:Veterinaria" "Server=localhost,1433;Database=VeterinariaDB;User Id=sa;Password=<tu-clave>;TrustServerCertificate=True;" --project Veterinaria.Api
dotnet user-secrets set "Jwt:Clave" "$(openssl rand -base64 48)" --project Veterinaria.Api
dotnet run --project Veterinaria.Api --launch-profile http
```
Swagger: http://localhost:5150/swagger (botón **Authorize**: pega el token del login) · Pruebas: `Veterinaria.Api/Veterinaria.Api.http`

Usuarios de prueba (clave `Clave123!`): `cliente@vetsalud.pe`, `recepcion@vetsalud.pe`, `veterinario@vetsalud.pe`.

## Scaffold (base de datos primero)
```bash
dotnet ef dbcontext scaffold "Name=ConnectionStrings:Veterinaria" Microsoft.EntityFrameworkCore.SqlServer \
  --project Veterinaria.Datos --startup-project Veterinaria.Api \
  --output-dir Models --context VeterinariaContext --no-onconfiguring --force
```
Las entidades se movieron a `Veterinaria.Entidades/Entidades` y el contexto a `Veterinaria.Datos/Contexto`. La entidad `Usuario` se agregó a mano (S14).
