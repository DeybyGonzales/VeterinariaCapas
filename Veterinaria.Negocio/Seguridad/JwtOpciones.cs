namespace Veterinaria.Negocio.Seguridad;

// Sección "Jwt" de la configuración. La Clave NO va en appsettings.json: se guarda con dotnet user-secrets.
public class JwtOpciones
{
    public string Clave { get; set; } = null!;
    public string Emisor { get; set; } = null!;
    public string Audiencia { get; set; } = null!;
    public int MinutosExpiracion { get; set; } = 60;
}
