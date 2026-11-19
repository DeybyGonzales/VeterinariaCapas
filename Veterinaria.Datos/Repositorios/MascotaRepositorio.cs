using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Contexto;
using Veterinaria.Entidades.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class MascotaRepositorio : IMascotaRepositorio
{
    private readonly VeterinariaContext _context;

    public MascotaRepositorio(VeterinariaContext context) => _context = context;

    public async Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId, string? buscar, int? edadMax)
    {
        var consulta = _context.Mascotas.Include(m => m.Propietario).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(m => m.Especie == especie);
        if (propietarioId != null)
            consulta = consulta.Where(m => m.PropietarioId == propietarioId);
        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(m => EF.Functions.Collate(m.Nombre, "Latin1_General_CI_AI").Contains(buscar));
        if (edadMax != null)
            consulta = consulta.Where(m => m.Edad <= edadMax);

        return await consulta.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<List<Mascota>> ListarPorPropietarioAsync(int propietarioId) =>
        await _context.Mascotas
            .Include(m => m.Propietario)
            .AsNoTracking()
            .Where(m => m.PropietarioId == propietarioId)
            .OrderBy(m => m.Nombre)
            .ToListAsync();

    public async Task<Mascota?> ObtenerPorIdAsync(int id) =>
        await _context.Mascotas.Include(m => m.Propietario).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<bool> ExisteNombreParaPropietarioAsync(int propietarioId, string nombre, int? excluirId = null) =>
        await _context.Mascotas.AnyAsync(m => m.PropietarioId == propietarioId && m.Nombre == nombre
                                           && (excluirId == null || m.Id != excluirId));

    public async Task AgregarAsync(Mascota mascota) => await _context.Mascotas.AddAsync(mascota);

    public void Eliminar(Mascota mascota) => _context.Mascotas.Remove(mascota);

    public async Task GuardarAsync() => await _context.SaveChangesAsync();
}
