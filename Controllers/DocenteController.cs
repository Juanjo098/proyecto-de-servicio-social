using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.VisualBasic;

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

        [Authorize(Roles = "1")]
        [Route("/Administracion/Docentes/Insertar")]
        [HttpPost]
        public async Task<IActionResult> Insertar(Clases.Post.Docente modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.departamentos = await ListaDepartamentos();
                ViewBag.id_dpto = modelo.Id_Dpto;
                ViewBag.nombre = modelo.Nombre;
                ViewBag.titulo = modelo.Titulo;
                ViewBag.diminutivo = modelo.Diminutivo;
                ViewBag.cedula = modelo.Cedula;
                return View();
            }

            try
            {
                if (await Existe(modelo.Cedula))
                {
                    ViewBag.error = true;
                    ViewBag.id_dpto = modelo.Id_Dpto;
                    ViewBag.nombre = modelo.Nombre;
                    ViewBag.titulo = modelo.Titulo;
                    ViewBag.diminutivo = modelo.Diminutivo;
                    ViewBag.cedula = modelo.Cedula;
                    ViewBag.departamentos = await ListaDepartamentos();
                    return View();
                }

                Docente docente = new Docente {
                    IdDpto = modelo.Id_Dpto,
                    Nombre = modelo.Nombre.ToUpper(),
                    Titulo = modelo.Titulo.ToUpper(),
                    Diminutivo = modelo.Diminutivo.ToUpper(),
                    Cedula = modelo.Cedula
                };

                _context.Docentes.Add(docente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Docentes");
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Docentes/Detalles")]
        [HttpGet]
        public async Task<IActionResult> Detalles(int? id)
        {
            return View(await DocenteDetalles(id?? 1));
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Docentes/Editar")]
        public async Task<IActionResult> Editar(int? id)
        {
            ViewBag.departamentos = await ListaDepartamentos();
            return View(await EditarDocente(id ?? 1));
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Docentes/Editar")]
        [HttpPost]
        public async Task<IActionResult> Editar(Clases.Put.Docente modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.departamentos = await ListaDepartamentos();
                return View(modelo.Id_Docente);
            }

            try
            {
                if (await Existe(modelo.Cedula))
                {
                    ViewBag.error = true;
                    ViewBag.departamentos = await ListaDepartamentos();
                    return View(modelo.Id_Docente);
                }

                Docente docente = await _context.Docentes.FindAsync(modelo.Id_Docente);

                if (docente == null)
                    return RedirectToAction("CustomError", "Home");

                docente.Nombre = modelo.Nombre.ToUpper();
                docente.Titulo = modelo.Titulo.ToUpper();
                docente.Diminutivo = modelo.Diminutivo.ToUpper();
                docente.Cedula = modelo.Cedula;
                docente.IdDpto = modelo.Id_Dpto;

                await _context.SaveChangesAsync();

                return RedirectToAction("Docentes");
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
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

        private async Task<bool> Existe(string cedula)
        {
            return await _context.Docentes.FirstOrDefaultAsync(doc => doc.Cedula == cedula) != null;
        }

        private async Task<Clases.Put.Docente> EditarDocente(int id)
        {
            Docente doc = await _context.Docentes.FindAsync(id);

            if (doc != null)
            {
                return new Clases.Put.Docente { 
                    Id_Docente = doc.IdDocente,
                    Id_Dpto = doc.IdDpto,
                    Nombre = doc.Nombre,
                    Titulo = doc.Titulo,
                    Diminutivo = doc.Diminutivo,
                    Cedula = doc.Cedula,
                };
            }

            return null;
        }
    }
}
