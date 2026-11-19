using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Contexto;
using Veterinaria.Entidades.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly VeterinariaContext _context;

    public UsuarioRepositorio(VeterinariaContext context) => _context = context;

    public Task<Usuario?> BuscarPorCorreoAsync(string correo) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

    public async Task AgregarAsync(Usuario usuario) => await _context.Usuarios.AddAsync(usuario);

    public Task GuardarAsync() => _context.SaveChangesAsync();
}
