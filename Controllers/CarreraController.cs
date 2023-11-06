using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Titulacion.Controllers
{
    public class CarreraController : Controller
    {
        private readonly TitulacionContext _context;
        public CarreraController(TitulacionContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1,2")]
        [Route("Administracion/Carreras")]
        public async Task<IActionResult> Carreras(int? numPag, string buscar)
        {
            int cantidad = 10;
            try
            {
                List<Clases.Get.Carrera> items = await ListaCarreras();

                if (!string.IsNullOrEmpty(buscar) && items != null)
                {
                    items = items.FindAll(item => item.Nombre.ToLower().Contains(buscar.ToLower()));
                    ViewBag.buscar = buscar;
                }

                var pag = Paginacion<Clases.Get.Carrera>.CrearLista(items, numPag ?? 1, cantidad);

                return View(pag);
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1,2")]
        [Route("Administracion/Carreras/Detalles")]
        public async Task<IActionResult> Detalles(int id)
        {
            try
            {
                Clases.Get.CarreraDetalles detalles = await DetallesCarrera(id);

                if(detalles == null)
                    return RedirectToAction("CustomError", "Home");

                return View(detalles);
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1")]
        [Route("Administracion/Carreras/Insertar")]
        public async Task<IActionResult> Insertar()
        {
            ViewBag.departamentos = await ListaDepartamentos(true);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("Administracion/Carreras/Insertar")]
        public async Task<IActionResult> Insertar(Clases.Post.Carrera modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.departamentos = await ListaDepartamentos(true);
                return View();
            }

            try
            {
                modelo.Nombre = modelo.Nombre.ToUpper();
                if (await ExisteCarrera(modelo.Nombre))
                {
                    ViewBag.errorCarrera = "El nombre de la carrera no puede repetirse";
                    ViewBag.departamentos = await ListaDepartamentos(true);
                    return View();
                }

                Carrera carrera = new Carrera { Nombre = modelo.Nombre, IdDpto = modelo.IdDpto };

                _context.Carreras.Add(carrera);
                await _context.SaveChangesAsync();

                return RedirectToAction("Carreras");
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1")]
        [Route("Administracion/Carreras/Editar")]
        public async Task<IActionResult> Editar(int id)
        {
            ViewBag.departamentos = await ListaDepartamentos(false);
            try {
                return View(await EditarCarrera(id));
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }

        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("Administracion/Carreras/Editar")]
        public async Task<IActionResult> Editar(Clases.Put.Carrera modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.departamentos = await ListaDepartamentos(false);
                return View();
            }

            try
            {
                modelo.Nombre = modelo.Nombre.ToUpper();
                if (await _context.Carreras.FirstOrDefaultAsync(car => car.Nombre == modelo.Nombre && car.IdCarrera != modelo.IdCarrera && car.Hab == 1) != null)
                {
                    ViewBag.departamentos = await ListaDepartamentos(false);
                    ViewBag.errorCarrera = "No puede haber dos carreras con el mismo nombre";
                    return View(modelo);
                }

                Carrera carrera = await _context.Carreras.FirstOrDefaultAsync(car => car.Hab == 1 && car.IdCarrera == modelo.IdCarrera);

                if ( carrera == null )
                    return RedirectToAction("CustomError", "Home");

                carrera.Nombre = modelo.Nombre.ToUpper();
                carrera.IdDpto = modelo.IdDpto;

                _context.Carreras.Update(carrera);
                await _context.SaveChangesAsync();

                return RedirectToAction("Carreras");

            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("Administracion/Carreras/Eliminar")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                Carrera carrera = await _context.Carreras.FirstOrDefaultAsync(c => c.IdCarrera == id);

                if (carrera == null )
                    return RedirectToAction("CustomError", "Home");

                carrera.Hab = 0;
                _context.Carreras.Update(carrera);
                await _context.SaveChangesAsync();

                return RedirectToAction("Carreras");
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("CustomError", "Home");
            }
        }

        //Utilidades
        private async Task<List<Clases.Get.Carrera>> ListaCarreras()
        {
            return await (
                    from carrera in _context.Carreras
                    where carrera.Hab == 1
                    select new Clases.Get.Carrera { IdCarrera = carrera.IdCarrera, Nombre = carrera.Nombre }
                ).ToListAsync();
        }

        private async Task<List<SelectListItem>> ListaDepartamentos(bool elegir)
        {
            try
            {
                List<SelectListItem> lista = await (
                        from departamento in _context.Departamentos
                        where departamento.Hab == 1
                        select new SelectListItem { Text = departamento.Nombre, Value = departamento.IdDpto.ToString()}
                    ).ToListAsync();
                if (elegir)
                    lista.Insert(0, new SelectListItem { Text = "Elige un departamento", Value = "0" });
                return lista;
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }

        private async Task<Clases.Get.CarreraDetalles> DetallesCarrera(int id)
        {
            var item = await (
                    from carrera in _context.Carreras
                    join departamento in _context.Departamentos
                    on carrera.IdDpto equals departamento.IdDpto
                    join docente in _context.Docentes
                    on departamento.IdJefeDpto equals docente.IdDocente
                    select new Clases.Get.CarreraDetalles {
                        IdCarrera = carrera.IdCarrera,
                        Nombre = carrera.Nombre,
                        Departamento = departamento.Nombre,
                        JefeDpto =  docente.Diminutivo + " " + docente.Nombre}
                ).ToListAsync();

            if (item != null)
                return item[0];

            return null;
        }

        private async Task<Clases.Put.Carrera> EditarCarrera(int id)
        {
            Carrera carrera = await _context.Carreras.FirstOrDefaultAsync(c => c.IdCarrera == id);
            
            if (carrera == null)
                return null;

            return new Clases.Put.Carrera { IdCarrera = carrera.IdCarrera, Nombre = carrera.Nombre, IdDpto = carrera.IdDpto };
        }

        private async Task<bool> ExisteCarrera(string nombre)
        {
            return await _context.Carreras.FirstOrDefaultAsync(c => c.Nombre == nombre)!= null;
        }
    }
}
