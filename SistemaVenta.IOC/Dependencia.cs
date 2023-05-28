using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.DAL.Repositorios;
using SistemaVenta.Utility;

namespace SistemaVenta.IOC
{
    public static class Dependencia
    {
        public static void inyectarDependencias(this IServiceCollection service, IConfiguration configuration) {
            service.AddDbContext<DbventaContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("cadenaSQL"));
            });

            service.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));//utiliza mdelo generico para cualquier odelo
            service.AddScoped<IVentaRepository, VentaRepository>();// aqui especificamos el modelo exacto o el alcance a que modelo

            //Dependencia de automapper
            service.AddAutoMapper(typeof(AutoMapperProfile));
        }
    }
}
