using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Titulacion.Controllers
{
    public class DocenteController : Controller
    {
        private readonly TitulacionContext _context;

        public DocenteController(TitulacionContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Docentes")]
        [HttpGet]
        public async Task<IActionResult> Docentes(int? numPag, string buscar)
        {
            int cantidad = 10;
            List<Clases.Get.Docente> items;
            try
            {
                items = await ListaDocentes();
            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction("CustomError", "Home");
            }

            if (!string.IsNullOrEmpty(buscar) && items != null)
            {
                items = items.FindAll(item => item.Nombre.ToLower().Contains(buscar.ToLower()));
                ViewBag.buscar = buscar;
            }

            var pag = Paginacion<Clases.Get.Docente>.CrearLista(items, numPag ?? 1, cantidad);
            return View(pag);
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Docentes/Insertar")]
        public async Task<IActionResult> Insertar()
        {
            ViewBag.departamentos = await ListaDepartamentos();
            return View();
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Docentes/Detalles")]
        [HttpGet]
        public async Task<IActionResult> Detalles(int? id)
        {
            return View(await DocenteDetalles(id?? 1));
        }


        // Utilidades

        private async Task<List<Clases.Get.Docente>> ListaDocentes() {
            return await (
                    from docente in _context.Docentes
                    where docente.Hab == 1
                    select new Clases.Get.Docente
                    {
                        Id = docente.IdDocente,
                        Nombre = docente.Nombre,
                        Cedula = docente.Cedula
                    }
                ).ToListAsync();
        }

        private async Task<Clases.Get.DocenteDetalles> DocenteDetalles(int id)
        {
            Clases.Get.DocenteDetalles docente;
            List<string> cargos;

            docente = await (
                    from doc in _context.Docentes
                    join dep in _context.Departamentos
                    on doc.IdDpto equals dep.IdDpto
                    where doc.Hab == 1 && dep.Hab == 1 && doc.IdDocente == id
                    select new Clases.Get.DocenteDetalles{
                        Id = doc.IdDocente,
                        Nombre = doc.Diminutivo + " " + doc.Nombre,
                        Titulo = doc.Titulo,
                        Cedula = doc.Cedula,
                        Departamento = dep.Nombre
                    }
                ).FirstOrDefaultAsync();

            cargos = await (
                    from inter in _context.DocenteCargos
                    join cargo in _context.Cargos
                    on inter.IdDocente equals docente.Id
                    select cargo.Nombre
                ).ToListAsync();

            docente.Cargos = cargos;
            docente.Cargos.Insert(0, "Docente");


            return docente;
        }

        private async Task<List<SelectListItem>> ListaDepartamentos()
        {
            List<SelectListItem> lista;
            lista = await (
                from dpto in _context.Departamentos
                where dpto.Hab == 1
                select new SelectListItem { Text = dpto.Nombre, Value = dpto.IdDpto.ToString()}
                ).ToListAsync();
            lista.Insert(0, new SelectListItem { Text = "Selecciona el departamento", Value = "0" });
            return lista;
        }
    }
}
