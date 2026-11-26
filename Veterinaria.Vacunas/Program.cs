using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Vacunas.Modelos;

var builder = WebApplication.CreateBuilder(args);

// ---------- JWT ----------
// Misma clave, emisor y audiencia que Veterinaria.Api (S14).
// La Clave NO está en el repositorio: user-secrets en local, variable de entorno Jwt__Clave en la nube.
var emisor = builder.Configuration["Jwt:Emisor"] ?? throw new InvalidOperationException("Falta Jwt:Emisor.");
var audiencia = builder.Configuration["Jwt:Audiencia"] ?? throw new InvalidOperationException("Falta Jwt:Audiencia.");
var clave = builder.Configuration["Jwt:Clave"];
if (string.IsNullOrEmpty(clave) || Encoding.UTF8.GetByteCount(clave) < 32)
    throw new InvalidOperationException("Jwt:Clave debe tener al menos 32 caracteres (usa dotnet user-secrets).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = emisor,
            ValidateAudience = true,
            ValidAudience = audiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            NameClaimType = "email",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Todos los endpoints del microservicio exigen un token válido (cualquier rol)
var vacunas = app.MapGroup("/api/vacunas").RequireAuthorization();

// GET api/vacunas?especie=Gato   -> lista (filtro opcional por especie)
vacunas.MapGet("/", (string? especie) =>
{
    var lista = string.IsNullOrWhiteSpace(especie)
        ? VacunaDatos.Lista
        : VacunaDatos.Lista.Where(v =>
              v.Especie.Equals(especie, StringComparison.OrdinalIgnoreCase) || v.Especie == "Ambos").ToList();
    return Results.Ok(lista);
});

// GET api/vacunas/5   -> una vacuna o 404
vacunas.MapGet("/{id:int}", (int id) =>
{
    var vacuna = VacunaDatos.Lista.FirstOrDefault(v => v.Id == id);
    return vacuna is null
        ? Results.NotFound(new { mensaje = $"No existe la vacuna con id {id}." })
        : Results.Ok(vacuna);
});

app.MapGet("/", () => "Veterinaria.Vacunas - microservicio de vacunas (usa /api/vacunas con token)");

app.Run();
