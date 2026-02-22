using eVote360.Core.Application.Interface;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Application.Interface.Votacion;
using eVote360.Core.Application.Services.Admin;
using eVote360.Core.Application.Services.Dirigente;
using eVote360.Core.Application.Services.Elecciones;
using eVote360.Core.Application.Services.Votacion;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360.Core.Application
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            #region Services IOC
            services.AddTransient<IAsignacionDirigenteService, AsignacionDirigenteService>();
            services.AddTransient<ICiudadanoService, CiudadanoService>();
            services.AddTransient<IEleccionService, EleccionService>();
            services.AddTransient<IAlianzaPoliticaService, AlianzaPoliticaService>();
            services.AddTransient<IAsignacionCandidatoPuestoService, AsignacionCandidatoPuestoService>();
            services.AddTransient<ICandidatoService, CandidatoService>();
            services.AddTransient<IElectorService, ElectorService>();
            services.AddTransient<IResultadosService, ResultadosService>();
            services.AddTransient<IPuestoElectivoService, PuestoElectivoService>();
            services.AddTransient<IPartidoService, PartidoService>();
            services.AddTransient<IUsuarioService, UsuarioService>();

            #endregion
        }
    }
}
