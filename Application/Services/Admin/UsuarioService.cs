using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using eVote360.Core.Application.Dtos.Admin.Usuario;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;

namespace eVote360.Core.Application.Services.Admin
{
    public sealed class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;
        private readonly IDirigentePartidoRepository _dirigentePartidoRepo;

        public UsuarioService(IUsuarioRepository repo, IEleccionRepository eleccionRepo, IMapper mapper, IDirigentePartidoRepository dirigentePartidoRepo)
        {
            _repo = repo;
            _eleccionRepo = eleccionRepo;
            _mapper = mapper;
            _dirigentePartidoRepo = dirigentePartidoRepo ??
                        throw new ArgumentNullException(nameof(dirigentePartidoRepo));



        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
        public async Task<UsuarioDto?> LoginAsync(LoginDto dto)
        {
            string hashed = Hash(dto.PasswordHash);
            Usuario? user = await _repo.LoginAsync(dto.UserName, hashed);

            if (user == null)
            {
                return null;
            }

            UsuarioDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Apellido = user.Apellido,
                Nombre = user.Nombre,
                Rol = user.Rol,
                UserName = user.UserName,
                IsActive = user.IsActive,
            };

            return userDto;
        }

        public async Task<int?> GetPartidoAsignadoAsync(int usuarioId, CancellationToken ct = default)
        {
           
            var asignacion = (await _dirigentePartidoRepo.FindAsync(
                x => x.UsuarioId == usuarioId && x.IsActive, ct
            )).FirstOrDefault();

            

            if (asignacion is null)
                return null;

            return asignacion.PartidoId;
        }


        public async Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var ent = await _repo.GetByIdAsync(id, ct);
            return ent is null ? null : _mapper.Map<UsuarioDto>(ent);
        }

        public async Task<List<UsuarioDto>> GetAllAsync(CancellationToken ct)
        {
            var all = await _repo.GetAllListAsync(ct);
            return all.Select(_mapper.Map<UsuarioDto>).ToList();
        }

        public async Task<int> CreateAsync(UsuarioCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede crear usuario con elección activa.");

            if (await _repo.AnyAsync(u => u.UserName == dto.UserName, ct))
                throw new InvalidOperationException("El nombre de usuario ya existe.");

            var ent = _mapper.Map<Usuario>(dto);
            ent.IsActive = true;
            ent.PasswordHash = Hash(dto.Password);

            var saved = await _repo.AddAsync(ent, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear el usuario.");
            return saved.Id;
        }

        public async Task<bool> UpdateAsync(UsuarioUpdateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede editar usuario con elección activa.");

            var current = await _repo.GetByIdAsync(dto.Id, ct);
            if (current is null) return false;

            if (await _repo.AnyAsync(u => u.Id != dto.Id && u.UserName == dto.UserName, ct))
                throw new InvalidOperationException("Otro usuario ya tiene ese nombre de usuario.");

            current.Nombre = dto.Nombre;
            current.Apellido = dto.Apellido;
            current.Email = dto.Email;
            current.UserName = dto.UserName;
            current.Rol = dto.Rol;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                current.PasswordHash = Hash(dto.Password);

            return await _repo.UpdateAsync(current.Id, current, ct) is not null;
        }

        public async Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede cambiar estado del usuario con elección activa.");

            var current = await _repo.GetByIdAsync(id, ct);
            if (current is null) return false;

            current.IsActive = active;
            return await _repo.UpdateAsync(id, current, ct) is not null;
        }
    }
}
