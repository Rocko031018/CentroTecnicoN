﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Drawing;

using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.AplicacionWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaService;
        private readonly IVentaService _ventaService;
        private readonly IMapper _mapper;
        private readonly SistemaVenta.AplicacionWeb.Models.DbventaContext _context;
        public VentaController(ITipoDocumentoVentaService tipoDocumentoVentaService, IVentaService ventaService, IMapper mapper, SistemaVenta.AplicacionWeb.Models.DbventaContext context)
        {
            _tipoDocumentoVentaService = tipoDocumentoVentaService;
            _ventaService = ventaService;
            _mapper = mapper;
            _context = context;
        }
        public async Task<IActionResult> NuevaVenta()

        {
            var model = new RepairViewModel()
            {
                Repair = new Repair(),
                Clientes = await _context.Usuarios.Where(e => e.IdRol == 3).ToListAsync(),
                Brands = await _context.Brands.ToListAsync()

            };

            return View(model);
        }

        public async Task<IActionResult> HistorialVenta()
        {
         var list =  await _context.Repairs.Include(id => id.IdModelNavigation).Include(id => id.ClientNavigation).Include(
                id => id.IdBrandNavigation).ToListAsync();
            if (list is null) return NotFound();
             var listDTO = _mapper.Map<List<Models.ViewModels.VMRepair>>(list);
            return View(listDTO);
        }
        [HttpGet]
        public async Task<IActionResult> SearchForEMEI(string imei)
        {
            var dat = await _context.Repairs.Where(id => id.Imei == imei).FirstOrDefaultAsync();
            
            if (dat is null) return RedirectToAction("HistorialVenta", "Venta");
            var map = _mapper.Map<Models.ViewModels.VMRepair>(dat);
            return View(map);

        }
        [HttpPut]
        public async Task<IActionResult> Reparado(int idR)
        {
            var dat = await _context.Repairs.FirstOrDefaultAsync(id => id.IdRep == idR);
            if (dat is null) return NotFound();
            dat.Condition = "Reparado";
            _context.Repairs.Update(dat);
            await _context.SaveChangesAsync();
            return RedirectToAction("HistorialVenta", "Venta");

        }
        [HttpGet]
        public async Task<JsonResult> SelectBrand(int id)
        {
            var modelPhones = await _context.ModelPhones.Where(e => e.IdBrand == id).ToListAsync();

            var result = modelPhones.Select(mp => new
            {
                id = mp.IdModel,
                nombre = mp.ModelName
            
            });

            var model = new RepairViewModel
            {
                ModelPhones = modelPhones
            };

            return Json(result);
        }

        


        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            List<VMTipoDocumentoVenta> vmListaTipoDocumentos = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaService.ObtenerProductos(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
        {
            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();
            try
            {

                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                modelo.IdUsuario = int.Parse(idUsuario);
                Venta venta_creada = await _ventaService.Registrar(_mapper.Map<Venta>(modelo));
                modelo = _mapper.Map<VMVenta>(venta_creada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpGet] 
        public async Task<IActionResult> CrearReparacion()
        {
            var list = await _context.Usuarios.Where(e => e.IdRol == 3).ToListAsync();
            var model = new RepairViewModel
            {
                Repair = new Repair(),
            };

            // Supongamos que ObtenerClientes devuelve una lista de clientes
            var listaClientes = await _context.Usuarios.Where(e => e.IdRol == 3).ToListAsync();

            // Añade la lista de clientes al ViewBag
            ViewBag.ListaClientes = new SelectList(listaClientes, "IdUsuario", "Nombre");

            return View("NuevaVenta", model);



       
        }

        [HttpPost]
        public async Task< IActionResult> AgregarReparacion(RepairViewModel modelo)
        {
            if(modelo.Repair.IdModel > 0)
            {
                modelo.Repair.TipeEq = "Telefono";
                modelo.Repair.Condition = "En Reparación";
                _context.Add(modelo.Repair);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("HistorialVenta", "Venta");
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _ventaService.Historial(numeroVenta,fechaInicio,fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }
        
        public IActionResult MostrarPDFVenta(string numeroVenta)
        {
            string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFVenta?numeroVenta={numeroVenta}";
            var archivoPDF = "dos";

            return File(archivoPDF, "application/pdf");
        }
    }
}
