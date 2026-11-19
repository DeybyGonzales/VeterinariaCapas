using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Contexto;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio.Mapeos;
using Veterinaria.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);

// NumberHandling = Strict: los números son solo números (no "integer | string" en el documento OpenAPI)
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict);
// OpenAPI 3.0: con 3.1 los parámetros {id} salen como "integer | string" y Swagger UI no los acepta
builder.Services.AddOpenApi(o => o.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0);

// La cadena de conexión NO está en el repositorio: en local se guarda con dotnet user-secrets
var cadena = builder.Configuration.GetConnectionString("Veterinaria")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'Veterinaria' (usa dotnet user-secrets).");

// ---------- Inyección de dependencias ----------
// DbContext (capa Datos)
builder.Services.AddDbContext<VeterinariaContext>(options => options.UseSqlServer(cadena));

// Repositorios (capa Datos)
builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();

// Servicios (capa Negocio)
builder.Services.AddScoped<IPropietarioServicio, PropietarioServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();

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
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
