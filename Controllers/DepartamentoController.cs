using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;

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

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Departamentos/Detalles")]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                string nombreJefeDpto;
                
                Clases.Get.DepartamentoDetalles departamento = new Clases.Get.DepartamentoDetalles();
                
                Departamento dep = await _context.Departamentos.FirstOrDefaultAsync(d => d.IdDpto == id);
                
                List<string> carreras = await (
                        from c in _context.Carreras
                        join d in _context.Departamentos
                        on c.IdDpto equals d.IdDpto
                        where c.Hab == 1 && d.Hab == 1 && c.IdDpto == id
                        select c.Nombre).ToListAsync();

                if (dep == null) {
                    return RedirectToAction("CustomError", "Home");
                }

                nombreJefeDpto = await TituloDocente(dep.IdJefeDpto);

                if (nombreJefeDpto == null)
                    return RedirectToAction("CustomError", "Home");

                departamento.Id = dep.IdDpto;
                departamento.NombreDpto = dep.Nombre;
                departamento.NombreJefeDpto = nombreJefeDpto;
                departamento.Carreras = carreras;

                return View(departamento);
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
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
                modelo.Nombre = modelo.Nombre.ToUpper();
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
            ViewBag.docentes = await ListaDocentes();
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
                modelo.Nombre = modelo.Nombre.ToUpper();
                if (_context.Departamentos.FirstOrDefault(dep => dep.Nombre == modelo.Nombre && dep.IdDpto != modelo.Id) != null)
                {
                    ViewBag.docentes = await ListaDocentes();
                    ViewBag.errorNombreDpto = "No puede colocar un nombre de departamento que ya existe";
                    return View(modelo);
                }

                if (! await ExisteDocente(modelo.JefeDpto))
                {
                    ViewBag.docentes = await ListaDocentes();
                    ViewBag.errorDocente = "Elija un docente existente o deje el campo vacio";
                    return View(modelo);
                }

                Models.Departamento dep = await _context.Departamentos.FindAsync(modelo.Id);

                if (dep == null)
                {
                    ViewBag.docentes = await ListaDocentes();
                    ViewBag.errorID = "No se encontro un departamento con el id especificado";
                    return View(modelo);
                }

                dep.Nombre = modelo.Nombre;
                dep.IdJefeDpto = await IdDocente(modelo.JefeDpto);

                _context.Departamentos.Update(dep);
                await _context.SaveChangesAsync();

                return RedirectToAction("Departamentos", "Departamento");
            }
            catch (InvalidOperationException ex) {
                return RedirectToAction("CustomError", "Home");
            }
            
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

            return new Clases.Put.Departamento { Id = id, Nombre = item.Nombre, JefeDpto = await NombreDocente(item.IdJefeDpto) };
        }

        private async Task<bool> Existe(string nombre)
        {
            return await _context.Departamentos.FirstOrDefaultAsync(d => d.Nombre.ToUpper() == nombre.ToUpper()) != null;
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
            return await _context.Docentes.FirstOrDefaultAsync(doc => doc.Nombre.ToUpper() == nombre.ToUpper()) != null;
        }

        private async Task<int?> IdDocente(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                return null;
            Models.Docente docente = await _context.Docentes.FirstOrDefaultAsync( doc => doc.Nombre == nombre);
            return docente == null ? null : docente.IdDocente;
        }

        private async Task<string?> NombreDocente (int? id)
        {
            if (id == null)
                return "";
            
            Docente docente = await _context.Docentes.FirstOrDefaultAsync(d => d.IdDocente == id);

            if (docente == null)
                return null;

            return docente.Nombre;
        }

        private async Task<string?> TituloDocente(int? id)
        {
            if (id == null)
                return "";

            Docente docente = await _context.Docentes.FirstOrDefaultAsync(d => d.IdDocente == id);

            if (docente == null)
                return null;

            return docente.Diminutivo + " " + docente.Nombre;
        }
    }
}
