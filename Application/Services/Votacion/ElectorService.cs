using AutoMapper;
using eVote360.Core.Application.Dtos.Admin.PuestoElectivo;
using eVote360.Core.Application.Dtos.Elecciones;
using eVote360.Core.Application.Dtos.Votacion;
using eVote360.Core.Application.Interface.Common;
using eVote360.Core.Application.Interface.Votacion;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Core.Application.Services.Votacion
{
    public sealed class ElectorService : IElectorService
    {
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IEleccionPuestoRepository _eleccionPuestoRepo;
        private readonly IEleccionCandidaturaRepository _eleccionCandidaturaRepo;
        private readonly ICiudadanoRepository _ciudadanoRepo;
        private readonly IPuestoElectivoRepository _puestoRepo;
        private readonly ICandidaturaRepository _candidaturaRepo;
        private readonly IVotoRepository _votoRepo;
        private readonly IOcrService _ocr;
        private readonly IEmailSender _email;
        private readonly IMapper _mapper;

        public ElectorService(
            IEleccionRepository eleccionRepo,
            IEleccionPuestoRepository eleccionPuestoRepo,
            IEleccionCandidaturaRepository eleccionCandidaturaRepo,
            ICiudadanoRepository ciudadanoRepo,
            IPuestoElectivoRepository puestoRepo,
            ICandidaturaRepository candidaturaRepo,
            IVotoRepository votoRepo,
            IOcrService ocr,
            IEmailSender email,
            IMapper mapper)
        {
            _eleccionRepo = eleccionRepo;
            _eleccionPuestoRepo = eleccionPuestoRepo;
            _eleccionCandidaturaRepo = eleccionCandidaturaRepo;
            _ciudadanoRepo = ciudadanoRepo;
            _puestoRepo = puestoRepo;
            _candidaturaRepo = candidaturaRepo;
            _votoRepo = votoRepo;
            _ocr = ocr;
            _email = email;
            _mapper = mapper;
        }

        private async Task<Eleccion?> GetEleccionActivaAsync(CancellationToken ct)
            => (await _eleccionRepo.FindAsync(e => e.Estado == ElectionStatus.EnProceso, ct)).FirstOrDefault();

        public async Task<(bool PuedeVotar, string Mensaje, int? CiudadanoId)> PreCheckAsync(string documentoIdentidad, CancellationToken ct)
        {
            var eleccion = await GetEleccionActivaAsync(ct);
            if (eleccion is null)
                return (false, "No hay ningún proceso electoral en estos momentos.", null); 

            var ciudadano = (await _ciudadanoRepo.FindAsync(c => c.DocumentoIdentidad == documentoIdentidad, ct)).FirstOrDefault();
            if (ciudadano is null)
                return (false, "Ciudadano no encontrado.", null);

            if (!ciudadano.IsActive)
                return (false, "El ciudadano está inactivo.", ciudadano.Id); 

            var yaVoto = await _votoRepo.AnyAsync(v => v.EleccionId == eleccion.Id && v.CiudadanoId == ciudadano.Id, ct);
            if (yaVoto)
                return (false, "Ya ha ejercido su derecho al voto.", ciudadano.Id); 

            return (true, "Puede continuar a la validación de identidad.", ciudadano.Id);
        }

        public async Task<bool> ValidarIdentidadConOcrAsync(string documentoDigitado,Stream fotoCedulaFrontal,CancellationToken ct)
        {
            var extraido = await _ocr.ExtractDocumentoAsync(fotoCedulaFrontal, ct);

            if (string.IsNullOrWhiteSpace(extraido))
                return false;

            // 🔹 Normalizar (quitar todo lo que no sea número)
            string Normalizar(string s)
                => new string(s.Where(char.IsDigit).ToArray());

            var docNorm = Normalizar(documentoDigitado);
            var ocrNorm = Normalizar(extraido);

            // 🔥 Comparación flexible
            return ocrNorm.Contains(docNorm);
        }

        public async Task<List<PuestoElectivoDto>> ListarPuestosDeEleccionActivaAsync(CancellationToken ct)
        {
            var eleccion = await GetEleccionActivaAsync(ct) ?? throw new InvalidOperationException("No hay elección activa.");
            var eps = await _eleccionPuestoRepo.FindAsync(ep => ep.EleccionId == eleccion.Id, ct);
            var puestosIds = eps.Select(ep => ep.PuestoElectivoId).ToList();
            var puestos = await _puestoRepo.FindAsync(p => puestosIds.Contains(p.Id), ct);
            return puestos.Select(_mapper.Map<PuestoElectivoDto>).ToList(); 
        }

        public async Task<List<EleccionCandidaturaDto>> ListarCandidaturasPorPuestoEnActivaAsync(int puestoElectivoId, CancellationToken ct)
        {
            var eleccion = await GetEleccionActivaAsync(ct) ?? throw new InvalidOperationException("No hay elección activa.");
            var list = await _eleccionCandidaturaRepo.FindAsync(ec =>
                ec.EleccionId == eleccion.Id && ec.Candidatura.PuestoElectivoId == puestoElectivoId && ec.IsActive, ct);
            return list.Select(_mapper.Map<EleccionCandidaturaDto>).ToList();
        }

        public async Task<bool> EmitirVotoAsync(VotoCreateDto dto, CancellationToken ct)
        {
            // 🔹 1️⃣ Obtener la elección activa correctamente
            var eleccion = await GetEleccionActivaAsync(ct)
                ?? throw new InvalidOperationException("No hay elección activa.");

            if (eleccion.Estado != ElectionStatus.EnProceso)
                throw new InvalidOperationException("La elección no está activa.");

            // 🔹 2️⃣ Validar ciudadano
            var ciudadano = await _ciudadanoRepo.GetByIdAsync(dto.CiudadanoId, ct)
                ?? throw new InvalidOperationException("Ciudadano no existe.");

            if (!ciudadano.IsActive)
                throw new InvalidOperationException("Ciudadano inactivo.");

            // 🔹 3️⃣ Verificar si ya votó ese puesto en la elección activa
            var yaVotoEsePuesto = await _votoRepo.AnyAsync(v =>
                v.EleccionId == eleccion.Id &&
                v.CiudadanoId == dto.CiudadanoId &&
                v.PuestoElectivoId == dto.PuestoElectivoId,
                ct);

            if (yaVotoEsePuesto)
                throw new InvalidOperationException("Ya existe un voto emitido para ese puesto.");

            // 🔹 4️⃣ Validar candidatura si aplica
            if (dto.CandidaturaId.HasValue)
            {
                var ec = (await _eleccionCandidaturaRepo.FindAsync(x =>
                    x.EleccionId == eleccion.Id &&
                    x.CandidaturaId == dto.CandidaturaId.Value,
                    ct)).FirstOrDefault()
                    ?? throw new InvalidOperationException("La candidatura no pertenece a la elección.");

                var cand = await _candidaturaRepo.GetByIdAsync(ec.CandidaturaId, ct)
                    ?? throw new InvalidOperationException("Candidatura inválida.");

                if (cand.PuestoElectivoId != dto.PuestoElectivoId)
                    throw new InvalidOperationException("La candidatura no corresponde al puesto seleccionado.");
            }

            // 🔹 5️⃣ Crear voto usando la elección activa real
            var voto = new Voto
            {
                Id = 0,
                EleccionId = eleccion.Id, // 👈 YA NO USA dto.EleccionId
                CiudadanoId = dto.CiudadanoId,
                PuestoElectivoId = dto.PuestoElectivoId,
                CandidaturaId = dto.CandidaturaId,
                EsNinguno = dto.CandidaturaId is null,
                FechaEmisionUtc = DateTime.UtcNow,
                IsActive = true
            };

            return await _votoRepo.AddAsync(voto, ct) is not null;
        }

        public async Task<bool> FinalizarVotacionAsync(int ciudadanoId, CancellationToken ct)
        {
            var eleccion = await GetEleccionActivaAsync(ct)
                ?? throw new InvalidOperationException("No hay elección activa.");

            // 🔹 Verificar que haya votado todos los puestos
            var puestos = await _eleccionPuestoRepo.FindAsync(ep => ep.EleccionId == eleccion.Id, ct);
            var totalPuestos = puestos.Count;

            var votosDelCiudadano = await _votoRepo.FindAsync(v =>
                v.EleccionId == eleccion.Id &&
                v.CiudadanoId == ciudadanoId, ct);

            if (votosDelCiudadano.Select(v => v.PuestoElectivoId).Distinct().Count() != totalPuestos)
                throw new InvalidOperationException("Debe completar el voto para todos los puestos antes de finalizar.");

            var ciudadano = await _ciudadanoRepo.GetByIdAsync(ciudadanoId, ct)
                ?? throw new InvalidOperationException("Ciudadano no existe.");

            if (string.IsNullOrWhiteSpace(ciudadano.Email))
                throw new InvalidOperationException("El ciudadano no tiene un correo electrónico registrado.");

            // 🔹 Construcción de correo más profesional
            string subject = $"Resumen de votación - {eleccion.Nombre}";

            string body = $@"
        <h2>Resumen de votación</h2>
        <p><strong>Elección:</strong> {eleccion.Nombre}</p>
        <p><strong>Fecha:</strong> {eleccion.FechaRealizacion:yyyy-MM-dd}</p>
        <hr/>
        <ul>
            {string.Join("", votosDelCiudadano.Select(v =>
                        $"<li>Puesto ID: {v.PuestoElectivoId} - {(v.EsNinguno ? "Ninguno" : $"Candidatura ID: {v.CandidaturaId}")}</li>"
                    ))}
        </ul>
        <br/>
        <p>Gracias por ejercer su derecho al voto.</p>
    ";

            await _email.SendAsync(ciudadano.Email, subject, body, ct);

            return true;
        }
    }
}
