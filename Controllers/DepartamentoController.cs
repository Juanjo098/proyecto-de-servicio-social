﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Titulacion.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly TitulacionContext _context;

        public DepartamentoController(TitulacionContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Departamentos")]
        public async Task<IActionResult> Departamentos(int? numPag, string buscar)
        {
            int cantidad = 10;
            var items = await ListaDepartamentos();

            if (!string.IsNullOrEmpty(buscar) && items != null)
            {
                items = items.FindAll(item => item.nombre.ToLower().Contains(buscar.ToLower()));
                ViewBag.buscar = buscar;
            }

            var pag = Paginacion<Clases.Get.Departamento>.CrearLista(items, numPag ?? 1, cantidad);

            return View(pag);
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Departamentos/Insertar")]
        public async Task<IActionResult> Insertar()
        {
            ViewBag.docentes = await ListaDocentes();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("/Administracion/Departamentos/Insertar")]
        public async Task<IActionResult> Insertar(Clases.Post.Departamento modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.docentes = await ListaDocentes();
                return View();
            }

            try
            {
                if (await Existe(modelo.Nombre))
                {
                    ViewBag.error = "El nombre del departamento ya se encuentra registrado";
                    ViewBag.docentes = await ListaDocentes();
                    return View();
                }

                if (! await ExisteDocente(modelo.JefeDpto))
                {
                    ViewBag.errorDocente = "Deje el campo en blanco o elija un docente existente";
                    ViewBag.docentes = await ListaDocentes();
                    return View();
                }

                int? jefeDpeto = await IdDocente(modelo.JefeDpto);
                _context.Departamentos.Add(new Models.Departamento { Nombre = modelo.Nombre, IdJefeDpto = jefeDpeto });
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException e)
            {
                ViewBag.error = e.Message;
                return View();
            }
            catch (InvalidOperationException e)
            {
                ViewBag.error = e.Message;
                return View();
            }

            return RedirectToAction("Departamentos", "Departamento");
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Departamentos/Editar")]
        public async Task<IActionResult> Editar(int id)
        {
            return View(await EditarDepartamento(id));
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("/Administracion/Departamentos/Editar")]
        public async Task<IActionResult> Editar(Clases.Put.Departamento modelo)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                if (await Existe(modelo.Nombre))
                {
                    ViewBag.error = "No puede colocar un nombre de departamento que ya existe";
                    return View(modelo);
                }
            }
            catch (InvalidOperationException ex) {
                ViewBag.error = "Error";
                return View(modelo);
            }

            
            Models.Departamento dep = await _context.Departamentos.FindAsync(modelo.Id);

            if (dep == null)
            {
                ViewBag.error = "No se encontro un departamento con el id especificado";
                return View();
            }

            dep.Nombre = modelo.Nombre;

            try
            {
                _context.Departamentos.Update(dep);
                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.error = "No se encontro un departamento con el id especificado";
                return View();
            }

            return RedirectToAction("Departamentos", "Departamento");
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("/Administracion/Departamentos/Eliminar")]
        public async Task<IActionResult> Eliminar(int id)
        {
            Models.Departamento dep = await _context.Departamentos.FindAsync(id);

            if (dep != null)
            {
                try
                {
                    dep.Hab = 0;
                    await _context.SaveChangesAsync();
                }
                catch (InvalidOperationException ex)
                {
                    return RedirectToAction("Departamentos", "Departamento");
                }
            }

            return RedirectToAction("Departamentos", "Departamento");
        }

        // Utilidades

        private async Task<List<Clases.Get.Departamento>> ListaDepartamentos()
        {
            return await (from departamento in _context.Departamentos
                    where departamento.Hab == 1
                    select new Clases.Get.Departamento { id = departamento.IdDpto, nombre = departamento.Nombre }).ToListAsync();
        }

        private async Task<Clases.Put.Departamento> EditarDepartamento(int id)
        {
            var item = await _context.Departamentos.FindAsync(id);

            if (item == null)
                return null;

            return new Clases.Put.Departamento { Id = id, Nombre = item.Nombre };
        }

        private async Task<bool> Existe(string nombre)
        {
            return await _context.Departamentos.FirstOrDefaultAsync(d => d.Nombre == nombre) != null;
        }

        private async Task<List<string>> ListaDocentes()
        {
            return await (
                from docente in _context.Docentes
                where docente.Hab == 1
                select docente.Nombre
            ).ToListAsync();
        }

        private async Task<bool> ExisteDocente(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                return true;
            return await _context.Docentes.FirstOrDefaultAsync(doc => doc.Nombre == nombre) != null;
        }

        private async Task<int?> IdDocente(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                return null;
            Models.Docente docente = await _context.Docentes.FirstOrDefaultAsync( doc => doc.Nombre == nombre);
            return docente == null ? null : docente.IdDocente;
        }
    }
}
