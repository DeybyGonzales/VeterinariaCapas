using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Contexto;
using Veterinaria.Entidades.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class PropietarioRepositorio : IPropietarioRepositorio
{
    private readonly VeterinariaContext _context;

    public PropietarioRepositorio(VeterinariaContext context) => _context = context;

    public async Task<List<Propietario>> ListarAsync(string? buscar)
    {
        var consulta = _context.Propietarios.Include(p => p.Mascotas).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
            // Collate "AI" = insensible a tildes (lopez encuentra López)
            consulta = consulta.Where(p => EF.Functions.Collate(p.Nombres, "Latin1_General_CI_AI").Contains(buscar)
                                        || p.Dni.Contains(buscar));

        return await consulta.OrderBy(p => p.Nombres).ToListAsync();
    }

    public async Task<Propietario?> ObtenerPorIdAsync(int id) =>
        await _context.Propietarios.Include(p => p.Mascotas).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<bool> ExisteAsync(int id) =>
        await _context.Propietarios.AnyAsync(p => p.Id == id);

    public async Task<bool> ExisteDniAsync(string dni, int? excluirId = null) =>
        await _context.Propietarios.AnyAsync(p => p.Dni == dni && (excluirId == null || p.Id != excluirId));

    public async Task<bool> TieneMascotasAsync(int id) =>
        await _context.Mascotas.AnyAsync(m => m.PropietarioId == id);

    public async Task AgregarAsync(Propietario propietario) =>
        await _context.Propietarios.AddAsync(propietario);

    public void Eliminar(Propietario propietario) => _context.Propietarios.Remove(propietario);

    public async Task GuardarAsync() => await _context.SaveChangesAsync();
}
