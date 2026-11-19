using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Entidades.Entidades;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public class MascotaServicio : IMascotaServicio
{
    private readonly IMascotaRepositorio _mascotas;
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMapper _mapper;

    public MascotaServicio(IMascotaRepositorio mascotas, IPropietarioRepositorio propietarios, IMapper mapper)
    {
        _mascotas = mascotas;
        _propietarios = propietarios;
        _mapper = mapper;
    }

    public async Task<List<MascotaDto>> ListarAsync(string? especie, int? propietarioId, string? buscar, int? edadMax) =>
        _mapper.Map<List<MascotaDto>>(await _mascotas.ListarAsync(especie, propietarioId, buscar, edadMax));

    public async Task<MascotaDto?> ObtenerPorIdAsync(int id)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        return mascota == null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        await ValidarAsync(dto.PropietarioId, dto.Nombre, null);

        var mascota = _mapper.Map<Mascota>(dto);
        await _mascotas.AgregarAsync(mascota);
        await _mascotas.GuardarAsync();

        // Se vuelve a leer con su propietario para devolver el nombre en el DTO
        var creada = await _mascotas.ObtenerPorIdAsync(mascota.Id);
        return _mapper.Map<MascotaDto>(creada);
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        if (mascota == null) return false;

        await ValidarAsync(dto.PropietarioId, dto.Nombre, id);

        _mapper.Map(dto, mascota);   // actualiza el objeto existente
        await _mascotas.GuardarAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        if (mascota == null) return false;

        _mascotas.Eliminar(mascota);
        await _mascotas.GuardarAsync();
        return true;
    }

    private async Task ValidarAsync(int propietarioId, string nombre, int? excluirId)
    {
        if (!await _propietarios.ExisteAsync(propietarioId))
            throw new DatoInvalidoException(nameof(MascotaCrearDto.PropietarioId), "El propietario indicado no existe.");

        // Regla de negocio 3: un propietario no puede tener dos mascotas con el mismo nombre
        if (await _mascotas.ExisteNombreParaPropietarioAsync(propietarioId, nombre, excluirId))
            throw new ReglaNegocioException($"El propietario ya tiene una mascota llamada {nombre}.");
    }
}
