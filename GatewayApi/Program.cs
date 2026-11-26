using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------- Rutas de Ocelot ----------
// En local se usa ocelot.json (localhost); publicado en la nube (Production) se usa ocelot.Production.json (hosts públicos)
var archivoOcelot = builder.Environment.IsProduction() ? "ocelot.Production.json" : "ocelot.json";
builder.Configuration.AddJsonFile(archivoOcelot, optional: false, reloadOnChange: true);

// ---------- JWT ----------
// El gateway valida el token ANTES de reenviar. Misma clave, emisor y audiencia que los dos servicios.
// La Clave NO está en el repositorio: user-secrets en local, variable de entorno Jwt__Clave en la nube.
var emisor = builder.Configuration["Jwt:Emisor"] ?? throw new InvalidOperationException("Falta Jwt:Emisor.");
var audiencia = builder.Configuration["Jwt:Audiencia"] ?? throw new InvalidOperationException("Falta Jwt:Audiencia.");
var clave = builder.Configuration["Jwt:Clave"];
if (string.IsNullOrEmpty(clave) || Encoding.UTF8.GetByteCount(clave) < 32)
    throw new InvalidOperationException("Jwt:Clave debe tener al menos 32 caracteres (usa dotnet user-secrets).");

// Esquema "Bearer": es el que se nombra en AuthenticationOptions de ocelot.json
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave))
        };
    });

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

await app.UseOcelot();   // Ocelot atiende todas las peticiones según las rutas configuradas

await app.RunAsync();
