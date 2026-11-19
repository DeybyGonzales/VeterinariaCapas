using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _servicio;

    public AuthController(IAuthServicio servicio) => _servicio = servicio;

    // POST api/auth/registro   (público). El rol NO viene del cliente: se asigna "Cliente".
    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<ActionResult<UsuarioDto>> Registro(RegistroDto dto)
    {
        try
        {
            var usuario = await _servicio.RegistrarAsync(dto);
            return StatusCode(StatusCodes.Status201Created, usuario);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // POST api/auth/login   (público). 401 igual si el correo no existe o la clave es incorrecta.
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var token = await _servicio.LoginAsync(dto);
        if (token == null) return Unauthorized(new { mensaje = "Correo o clave incorrectos." });
        return Ok(token);
    }

    // GET api/auth/perfil   (solo autenticados). Los datos salen de los claims del token, no de la BD.
    [HttpGet("perfil")]
    [Authorize]
    public ActionResult<PerfilDto> Perfil() => Ok(new PerfilDto
    {
        Correo = User.Claims.First(c => c.Type == "email").Value,
        Rol = User.Claims.First(c => c.Type == "role").Value
    });
}
