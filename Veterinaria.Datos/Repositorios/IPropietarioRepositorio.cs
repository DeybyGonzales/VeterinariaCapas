using Veterinaria.Entidades.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IPropietarioRepositorio
{
    Task<List<Propietario>> ListarAsync(string? buscar);
    Task<Propietario?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteDniAsync(string dni, int? excluirId = null);
    Task<bool> TieneMascotasAsync(int id);
    Task AgregarAsync(Propietario propietario);
    void Eliminar(Propietario propietario);
    Task GuardarAsync();
}
