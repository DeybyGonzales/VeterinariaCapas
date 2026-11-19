using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Entidades.Entidades;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Seguridad;

namespace Veterinaria.Negocio.Servicios;

public class AuthServicio : IAuthServicio
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IMapper _mapper;
    private readonly JwtOpciones _jwt;

    public AuthServicio(IUsuarioRepositorio usuarios, IMapper mapper, JwtOpciones jwt)
    {
        _usuarios = usuarios;
        _mapper = mapper;
        _jwt = jwt;
    }

    public async Task<UsuarioDto> RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();

        // Regla de negocio: el correo es único
        if (await _usuarios.BuscarPorCorreoAsync(correo) != null)
            throw new ReglaNegocioException($"Ya existe un usuario con el correo {correo}.");

        var usuario = new Usuario
        {
            Correo = correo,
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave),   // hash con sal, nunca la clave en texto
            Rol = Roles.PorDefecto,                                  // el rol lo decide el servidor
            FechaRegistro = DateTime.UtcNow
        };
        await _usuarios.AgregarAsync(usuario);
        await _usuarios.GuardarAsync();
        return _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarios.BuscarPorCorreoAsync(dto.Correo.Trim().ToLowerInvariant());

        // Misma respuesta (null -> 401) si el correo no existe o si la clave no coincide,
        // para no revelar qué correos están registrados.
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
            return null;

        return GenerarToken(usuario);
    }

    private TokenDto GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("role", usuario.Rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Clave));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddMinutes(_jwt.MinutosExpiracion);

        var token = new JwtSecurityToken(
            issuer: _jwt.Emisor,
            audience: _jwt.Audiencia,
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return new TokenDto { Token = new JwtSecurityTokenHandler().WriteToken(token), Expira = expira };
    }
}
