using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {

        private readonly DbventaContext _dbContext;


        public VentaRepository(DbventaContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }    
        public async Task<Venta> Registrar(Venta entidad)
        {
           Venta ventaGenerada = new Venta();

            using (var Transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    foreach(DetalleVenta dv in entidad.DetalleVenta)
                    {
                          //disminuir stock en la tabla
                          Producto producto_Encontrado= _dbContext.Productos.Where(p=>p.IdProducto==dv.IdProducto).First();

                        producto_Encontrado.Stock = producto_Encontrado.Stock - dv.Cantidad;
                        _dbContext.Productos.Update(producto_Encontrado);
                    }
                    //garda operaciones realizadas
                    await _dbContext.SaveChangesAsync();


                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n=>n.Gestion == "venta").First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion=DateTime.Now; 

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeronVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeronVenta = numeronVenta.Substring(numeronVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeronVenta;

                    await _dbContext.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;
                    Transaction.Commit();

                }catch (Exception ex) {
                Transaction.Rollback();  //restaura desde 0
                    throw ex;
                }

            }

            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date &&
                dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date).ToListAsync();

            return listaResumen;
                

        }
    }
}

