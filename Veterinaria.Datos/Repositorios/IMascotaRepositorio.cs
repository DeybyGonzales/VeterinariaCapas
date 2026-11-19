using Veterinaria.Entidades.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IMascotaRepositorio
{
    Task<List<Mascota>> ListarAsync(string? especie, int? propietarioId, string? buscar, int? edadMax);
    Task<List<Mascota>> ListarPorPropietarioAsync(int propietarioId);
    Task<Mascota?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreParaPropietarioAsync(int propietarioId, string nombre, int? excluirId = null);
    Task AgregarAsync(Mascota mascota);
    void Eliminar(Mascota mascota);
    Task GuardarAsync();
}
