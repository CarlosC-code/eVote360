using eVote360.Core.Application.Interface.Common;
using eVote360.Core.Domain.Interfaces;
using eVote360.Infrastructure.Persistence.Contexts;
using eVote360.Infrastructure.Persistence.Repositories;
using eVote360.Infrastructure.Persistence.Shared.Ocr;
using eVote360.Infrastructure.Shared.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360.Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Contexts
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<VoteAppContext>(opt =>
                                              opt.UseInMemoryDatabase("AppDb"));
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                services.AddDbContext<VoteAppContext>(opt =>
                opt.UseSqlServer(connectionString,
                m=> m.MigrationsAssembly(typeof(VoteAppContext).Assembly.FullName))
                , ServiceLifetime.Transient);
            }
            #endregion

            #region Repositories IOC
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IAlianzaRepository, AlianzaRepository>();
            services.AddTransient<IAlianzaSolicitudRepository, AlianzaSolicitudRepository>();
            services.AddTransient<ICandidatoRepository, CandidatoRepository>();
            services.AddTransient<ICandidaturaRepository, CandidaturaRepository>();
            services.AddTransient<ICiudadanoRepository, CiudadanoRepository>();
            services.AddTransient<IDirigentePartidoRepository, DirigentePartidoRepository>();
            services.AddTransient<IEleccionCandidaturaRepository, EleccionCandidaturaRepository>();
            services.AddTransient<IEleccionPuestoRepository, EleccionPuestoRepository>();
            services.AddTransient<IEleccionRepository, EleccionRepository>();
            services.AddTransient<IPartidoRepository, PartidoRepository>();
            services.AddTransient<IPuestoElectivoRepository, PuestoElectivoRepository>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IVotoRepository, VotoRepository>();
            services.AddSingleton<IOcrService, DevOcrService>();   
            services.AddTransient<IEmailSender, SmtpEmailSender>();
            #endregion
        }
    }
}
