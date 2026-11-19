using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.DTOs;

// Login: correo y clave
public class LoginDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La clave es obligatoria.")]
    public string Clave { get; set; } = null!;
}

// Registro: correo y clave. NO tiene rol: el rol lo asigna el servidor.
public class RegistroDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100)]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La clave es obligatoria.")]
    [StringLength(64, MinimumLength = 8, ErrorMessage = "La clave debe tener entre 8 y 64 caracteres.")]
    public string Clave { get; set; } = null!;
}

// Respuesta del login: el token y su fecha de expiración (UTC)
public class TokenDto
{
    public string Token { get; set; } = null!;
    public DateTime Expira { get; set; }
}

// Respuesta del registro (sin la clave ni el hash)
public class UsuarioDto
{
    public int Id { get; set; }
    public string Correo { get; set; } = null!;
    public string Rol { get; set; } = null!;
}

// Respuesta de GET api/auth/perfil: datos tomados de los claims del token
public class PerfilDto
{
    public string Correo { get; set; } = null!;
    public string Rol { get; set; } = null!;
}
