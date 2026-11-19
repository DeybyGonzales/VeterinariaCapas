using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Entidades.Entidades;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public class PropietarioServicio : IPropietarioServicio
{
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMascotaRepositorio _mascotas;
    private readonly IMapper _mapper;

    public PropietarioServicio(IPropietarioRepositorio propietarios, IMascotaRepositorio mascotas, IMapper mapper)
    {
        _propietarios = propietarios;
        _mascotas = mascotas;
        _mapper = mapper;
    }

    public async Task<List<PropietarioDto>> ListarAsync(string? buscar) =>
        _mapper.Map<List<PropietarioDto>>(await _propietarios.ListarAsync(buscar));

    public async Task<PropietarioDto?> ObtenerPorIdAsync(int id)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        return propietario == null ? null : _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<List<MascotaDto>?> ListarMascotasAsync(int id)
    {
        if (!await _propietarios.ExisteAsync(id)) return null;
        return _mapper.Map<List<MascotaDto>>(await _mascotas.ListarPorPropietarioAsync(id));
    }

    public async Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto)
    {
        // Regla de negocio 1: el DNI no puede repetirse
        if (await _propietarios.ExisteDniAsync(dto.Dni))
            throw new ReglaNegocioException($"Ya existe un propietario con el DNI {dto.Dni}.");

        var propietario = _mapper.Map<Propietario>(dto);
        await _propietarios.AgregarAsync(propietario);
        await _propietarios.GuardarAsync();
        return _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        if (propietario == null) return false;

        // Regla de negocio 1: el DNI no puede repetirse (sin contar al propio propietario)
        if (await _propietarios.ExisteDniAsync(dto.Dni, id))
            throw new ReglaNegocioException($"Ya existe otro propietario con el DNI {dto.Dni}.");

        _mapper.Map(dto, propietario);   // actualiza el objeto existente
        await _propietarios.GuardarAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        if (propietario == null) return false;

        // Regla de negocio 2: no se elimina un propietario que aún tiene mascotas
        if (await _propietarios.TieneMascotasAsync(id))
            throw new ReglaNegocioException("No se puede eliminar el propietario porque tiene mascotas registradas.");

        _propietarios.Eliminar(propietario);
        await _propietarios.GuardarAsync();
        return true;
    }
}
