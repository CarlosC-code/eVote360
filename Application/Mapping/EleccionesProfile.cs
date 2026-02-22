using AutoMapper;
using eVote360.Core.Application.Dtos.Elecciones;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Mapping;

public class EleccionesProfile : Profile
{
    public EleccionesProfile()
    {
        CreateMap<Eleccion, EleccionDto>().ReverseMap();
        CreateMap<EleccionCreateDto, Eleccion>();

        CreateMap<EleccionPuesto, EleccionPuestoDto>().ReverseMap();
        CreateMap<EleccionCandidatura, EleccionCandidaturaDto>().ReverseMap();
    }
}
