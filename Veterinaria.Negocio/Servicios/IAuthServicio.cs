using Veterinaria.Entidades.DTOs;

namespace Veterinaria.Negocio.Servicios;

public interface IAuthServicio
{
    // Registra un usuario con el rol por defecto. Lanza ReglaNegocioException si el correo ya existe.
    Task<UsuarioDto> RegistrarAsync(RegistroDto dto);

    // Devuelve el token, o null si el correo no existe o la clave es incorrecta.
    Task<TokenDto?> LoginAsync(LoginDto dto);
}
