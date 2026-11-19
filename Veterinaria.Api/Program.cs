using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Veterinaria.Datos.Contexto;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio.Mapeos;
using Veterinaria.Negocio.Seguridad;
using Veterinaria.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);

// NumberHandling = Strict: los números son solo números (no "integer | string" en el documento OpenAPI)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict);
// OpenAPI 3.0: con 3.1 los parámetros {id} salen como "integer | string" y Swagger UI no los acepta
builder.Services.AddOpenApi(o =>
{
    o.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

    // Esquema Bearer en el documento OpenAPI: habilita el botón "Authorize" de Swagger UI
    o.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Components ??= new OpenApiComponents();
        doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Pega solo el token (sin la palabra Bearer)."
        };
        doc.Security ??= new List<OpenApiSecurityRequirement>();
        doc.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", doc)] = new List<string>()
        });
        return Task.CompletedTask;
    });
});

// La cadena de conexión NO está en el repositorio: en local se guarda con dotnet user-secrets
var cadena = builder.Configuration.GetConnectionString("Veterinaria")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Veterinaria' (usa dotnet user-secrets).");

// ---------- JWT ----------
// Emisor, audiencia y minutos van en appsettings.json; la Clave en user-secrets (nunca en el repositorio)
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOpciones>()
    ?? throw new InvalidOperationException("Falta la sección 'Jwt' en la configuración.");
if (string.IsNullOrEmpty(jwt.Clave) || Encoding.UTF8.GetByteCount(jwt.Clave) < 32)
    throw new InvalidOperationException("Jwt:Clave debe tener al menos 32 caracteres (usa dotnet user-secrets).");
builder.Services.AddSingleton(jwt);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;   // conserva los nombres cortos: sub, email, role
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,       // vence exactamente a la hora indicada
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
            NameClaimType = "email",
            RoleClaimType = "role"           // [Authorize(Roles = ...)] lee este claim
        };
    });
builder.Services.AddAuthorization();

// ---------- Inyección de dependencias ----------
// DbContext (capa Datos)
builder.Services.AddDbContext<VeterinariaContext>(options => options.UseSqlServer(cadena));

// Repositorios (capa Datos)
builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Servicios (capa Negocio)
builder.Services.AddScoped<IPropietarioServicio, PropietarioServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();

// AutoMapper con el MappingProfile de la capa Negocio
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Veterinaria API v1");
        options.RoutePrefix = "swagger";
        options.EnablePersistAuthorization();   // el token sigue puesto al recargar
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();   // primero: ¿quién eres? (lee y valida el token)
app.UseAuthorization();    // después: ¿qué puedes hacer? (roles)
app.MapControllers();

app.Run();
