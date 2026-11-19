using AutoMapper;
using Veterinaria.Entidades.DTOs;
using Veterinaria.Entidades.Entidades;

namespace Veterinaria.Negocio.Mapeos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------- Propietario ----------
        // Entidad -> DTO de lectura (lo que devuelve la API)
        CreateMap<Propietario, PropietarioDto>()
            .ForMember(d => d.CantidadMascotas, o => o.MapFrom(s => s.Mascotas.Count));

        // DTO de entrada -> Entidad (crear)
        CreateMap<PropietarioCrearDto, Propietario>();

        // DTO de entrada -> Entidad existente (actualizar con Map(origen, destino))
        CreateMap<PropietarioActualizarDto, Propietario>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Mascotas, o => o.Ignore());

        // ---------- Mascota ----------
        // Entidad -> DTO de lectura: en vez del objeto Propietario se devuelve su nombre
        CreateMap<Mascota, MascotaDto>()
            .ForMember(d => d.Propietario, o => o.MapFrom(s => s.Propietario.Nombres));

        // DTO de entrada -> Entidad (crear)
        CreateMap<MascotaCrearDto, Mascota>()
            .ForMember(d => d.Propietario, o => o.Ignore());

        // DTO de entrada -> Entidad existente (actualizar)
        CreateMap<MascotaActualizarDto, Mascota>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Propietario, o => o.Ignore());
    }
}
