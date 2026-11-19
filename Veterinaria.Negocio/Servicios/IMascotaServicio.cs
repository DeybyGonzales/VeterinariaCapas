using Veterinaria.Entidades.DTOs;

namespace Veterinaria.Negocio.Servicios;

public interface IMascotaServicio
{
    Task<List<MascotaDto>> ListarAsync(string? especie, int? propietarioId, string? buscar, int? edadMax);
    Task<MascotaDto?> ObtenerPorIdAsync(int id);
    Task<MascotaDto> CrearAsync(MascotaCrearDto dto);
    Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
