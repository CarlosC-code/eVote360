using eVote360.Core.Application.Dtos.Admin.Ciudadano;

namespace eVote360.Core.Application.Interface.Admin
{
    public interface ICiudadanoService
    {
        Task<CiudadanoDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<CiudadanoDto?> GetByDocumentoAsync(string documento, CancellationToken ct);
        Task<List<CiudadanoDto>> GetAllAsync(CancellationToken ct);
        Task<int> CreateAsync(CiudadanoCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(CiudadanoUpdateDto dto, CancellationToken ct);
        Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct);
    }
}