using SistemaVenta.DAL.Repositorios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorios.Contrato;
using SistemaVenta.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SistemaVenta.DAL.Repositorios
{
    public class VentaRepository:GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbventaContext _dbcontext;
            public VentaRepository(DbventaContext dbcontext):base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                { 
                    foreach (DetalleVenta dv in modelo.DetalleVenta) {
                        Producto producto_encontrado = _dbcontext.Productos.Where(p=> p.IdProducto == dv.IdProducto).First();
                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;//Restar del stock a la entidad
                        _dbcontext.Productos.Update(producto_encontrado);//actusalizar Stock 
                    }
                    await _dbcontext.SaveChangesAsync();

                    //Generar numero de documento
                    NumeroDocumento correlativo = _dbcontext.NumeroDocumentos.First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;

                    _dbcontext.NumeroDocumentos.Update(correlativo);
                    await _dbcontext.SaveChangesAsync();

                    //Generar formato de numero de documento de Venta, ejm.(0001, 0002, 0003....)
                    int CantidadDigitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", CantidadDigitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    //00001 => 0001, siempre tendrá 4 digitos el numero de documento
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - CantidadDigitos, CantidadDigitos);
                    await _dbcontext.Venta.AddAsync(modelo);
                    await _dbcontext.SaveChangesAsync();

                    ventaGenerada = modelo;

                    transaction.Commit();// confirma que se pueden guardar los cambios

                }
                catch
                {
                    transaction.Rollback();// si existe un erro esto devuelve todo a como estaba en la Base de datos
                    throw;// devolver error
                }

                return ventaGenerada;
            }
        }
    }
}
