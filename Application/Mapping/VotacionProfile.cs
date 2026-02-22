using AutoMapper;
using eVote360.Core.Application.Dtos.Votacion;
using eVote360.Core.Domain.Entities;

namespace eVote360.Core.Application.Mapping;

public class VotacionProfile : Profile
{
    public VotacionProfile()
    {
        CreateMap<VotoCreateDto, Voto>()
            .ForMember(d => d.EsNinguno, opt => opt.MapFrom(s => s.CandidaturaId == null))
            .ForMember(d => d.FechaEmisionUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
