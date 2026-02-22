using AutoMapper;
using eVote360.Core.Application.Dtos.Dirigente.Alianza;
using eVote360.Core.Application.Dtos.Dirigente.Candidato;
using eVote360.Core.Application.Dtos.Dirigente.Candidatura;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Mapping;

public class DirigenteProfile : Profile
{
    public DirigenteProfile()
    {
        // Candidato
        CreateMap<Candidato, CandidatoDto>().ReverseMap();
        CreateMap<CandidatoCreateDto, Candidato>();
        CreateMap<CandidatoUpdateDto, Candidato>();

        // Candidatura
        CreateMap<Candidatura, CandidaturaDto>().ReverseMap();
        CreateMap<CandidaturaCreateDto, Candidatura>();

        // Alianzas
        CreateMap<AlianzaSolicitud, AlianzaSolicitudDto>().ReverseMap();
        CreateMap<AlianzaSolicitudCreateDto, AlianzaSolicitud>()
            .ForMember(d => d.FechaSolicitudUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Alianza, AlianzaDto>().ReverseMap();
    }
}
