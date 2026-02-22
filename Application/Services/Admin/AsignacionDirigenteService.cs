using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Core.Application.Services.Admin
{
    public sealed class AsignacionDirigenteService : IAsignacionDirigenteService
    {
        private readonly IDirigentePartidoRepository _repo;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IPartidoRepository _partidoRepo;
        private readonly IEleccionRepository _eleccionRepo;

        public AsignacionDirigenteService(
            IDirigentePartidoRepository repo,
            IUsuarioRepository usuarioRepo,
            IPartidoRepository partidoRepo,
            IEleccionRepository eleccionRepo)
        {
            _repo = repo; _usuarioRepo = usuarioRepo; _partidoRepo = partidoRepo; _eleccionRepo = eleccionRepo;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<int> AsignarAsync(int usuarioId, int partidoId, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede crear asignación con elección activa.");

            var usuario = await _usuarioRepo.GetByIdAsync(usuarioId, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");
            if (usuario.Rol != UserRole.Dirigente || !usuario.IsActive)
                throw new InvalidOperationException("El usuario debe ser dirigente y estar activo.");

            var partido = await _partidoRepo.GetByIdAsync(partidoId, ct)
                          ?? throw new InvalidOperationException("Partido no encontrado.");
            if (!partido.IsActive)
                throw new InvalidOperationException("El partido debe estar activo.");

            if (await _repo.AnyAsync(x => x.UsuarioId == usuarioId, ct))
                throw new InvalidOperationException("Ese dirigente ya está asignado a un partido.");

            var rel = new DirigentePartido
            {
                Id = 0,                 
                UsuarioId = usuarioId,
                PartidoId = partidoId,
                IsActive = true
            };

            var saved = await _repo.AddAsync(rel, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear la asignación.");
            return saved.Id;
        }

        public async Task<bool> EliminarAsync(int asignacionId, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede eliminar asignación con elección activa.");

            // Eliminación lógica (en tu dominio, marcar IsActive=false)
            var current = await _repo.GetByIdAsync(asignacionId, ct);
            if (current is null) return false;

            current.IsActive = false;
            var updated = await _repo.UpdateAsync(asignacionId, current, ct);
            return updated is not null;
        }

        public async Task<List<(int AsignacionId, int UsuarioId, string UsuarioNombre, int PartidoId, string PartidoSiglas)>> ListarAsync(CancellationToken ct)
        {
            var query = _repo.GetAllQueryWithInclude(x => x.Usuario, x => x.Partido);
            var list = await query.AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.UsuarioId,
                    UsuarioNombre = x.Usuario.Nombre + " " + x.Usuario.Apellido,
                    x.PartidoId,
                    PartidoSiglas = x.Partido.Siglas
                })
                .ToListAsync(ct);

            return list.Select(x => (x.Id, x.UsuarioId, x.UsuarioNombre, x.PartidoId, x.PartidoSiglas)).ToList();
        }
    }
}
