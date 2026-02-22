using AutoMapper;
using eVote360.Core.Application.Dtos.Admin.Ciudadano;
using eVote360.Core.Application.Dtos.Admin.Partido;
using eVote360.Core.Application.Dtos.Admin.PuestoElectivo;
using eVote360.Core.Application.Dtos.Admin.Usuario;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Mapping;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        // Puesto Electivo
        CreateMap<PuestoElectivo, PuestoElectivoDto>().ReverseMap();
        CreateMap<PuestoElectivoCreateDto, PuestoElectivo>();
        CreateMap<PuestoElectivoUpdateDto, PuestoElectivo>();

        // Ciudadano
        CreateMap<Ciudadano, CiudadanoDto>().ReverseMap();
        CreateMap<CiudadanoCreateDto, Ciudadano>();
        CreateMap<CiudadanoUpdateDto, Ciudadano>();

        // Partido
        CreateMap<Partido, PartidoDto>().ReverseMap();
        CreateMap<PartidoCreateDto, Partido>();
        CreateMap<PartidoUpdateDto, Partido>();

        // Usuario
        CreateMap<Usuario, UsuarioDto>().ReverseMap();
        CreateMap<UsuarioCreateDto, Usuario>()
            .ForMember(d => d.PasswordHash, opt => opt.MapFrom(s => s.Password)); // en el servicio hashea
        CreateMap<UsuarioUpdateDto, Usuario>()
            .ForMember(d => d.PasswordHash, opt => opt.Condition(s => !string.IsNullOrWhiteSpace(s.Password)))
            .ForMember(d => d.PasswordHash, opt => opt.MapFrom(s => s.Password));
    }
}
