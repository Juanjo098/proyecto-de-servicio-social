using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;

namespace Titulacion.Controllers
{
    public class CargoController : Controller
    {
        private readonly TitulacionContext _context;

        public CargoController(TitulacionContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="1,2")]
        [Route("/Administracion/Cargos")]
        public async Task<IActionResult> Cargos(int? numPag, string buscar)
        {
            int cantidad = 10;
            var items = await ListaCargos();

            if (!string.IsNullOrEmpty(buscar) && items != null)
            {
                items = items.FindAll(item => item.Nombre.ToLower().Contains(buscar.ToLower()));
                ViewBag.buscar = buscar;
            }

            var pag = Paginacion<Clases.Get.Cargo>.CrearLista(items, numPag ?? 1, cantidad);
            return View(pag);
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Cargos/Insertar")]
        public IActionResult Insertar()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("/Administracion/Cargos/Insertar")]
        public async Task<IActionResult> Insertar(Clases.Post.Cargo modelo)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                modelo.Nombre = modelo.Nombre.ToUpper();
                if (await Existe(modelo.Nombre))
                {
                    ViewBag.error = "El nombre del cargo no puede estar repetido";
                    return View(modelo);
                }

                Cargo cargo = new Cargo { Nombre = modelo.Nombre };

                _context.Cargos.Add(cargo);
                await _context.SaveChangesAsync();

                return RedirectToAction("Cargos", "Cargo");
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.exception = true;
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Cargos/Editar")]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                Clases.Put.Cargo cargo = await EditarCargo(id);

                if (cargo == null)
                {
                    ViewBag.exception = true;
                    return RedirectToAction("Cargos");
                }

                return View(cargo);

            }
            catch (InvalidOperationException ex)
            {
                ViewBag.exception = true;
                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("/Administracion/Cargos/Editar")]
        public async Task<IActionResult> Editar(Clases.Put.Cargo modelo)
        {
            if (!ModelState.IsValid)
                return View();

            try
            {
                modelo.Nombre = modelo.Nombre.ToUpper();
                if (await Existe(modelo.Nombre))
                {
                    ViewBag.error = "El nombre del cargo no puede repetirse";
                    return View(modelo.Id);
                }

                Cargo cargo = await _context.Cargos.FindAsync(modelo.Id);

                if (cargo == null)
                {
                    ViewBag.exception = true;
                    return View();
                }

                cargo.Nombre = modelo.Nombre;
                await _context.SaveChangesAsync();

                return RedirectToAction("Cargos");

            }
            catch (InvalidOperationException ex)
            {
                ViewBag.exception = true;
                return View();
            }
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                Cargo cargo = await _context.Cargos.FindAsync(id);

                if (cargo == null)
                    return RedirectToAction("CustomError", "Home");

                cargo.Hab = 0;
                await _context.SaveChangesAsync();

                return RedirectToAction("Cargos");

            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("CustomError", "Home");
            }
            

        }

        // Utilidades
        private async Task<List<Clases.Get.Cargo>> ListaCargos()
        {
            return await (
                    from cargo in _context.Cargos
                    where cargo.Hab == 1
                    select new Clases.Get.Cargo { Id = cargo.IdCargo, Nombre = cargo.Nombre}
                ).ToListAsync();
        }

        private async Task<bool> Existe(string nombre)
        {
            return await _context.Cargos.FirstOrDefaultAsync(c => c.Nombre == nombre) != null;
        }

        private async Task<Clases.Put.Cargo> EditarCargo(int id)
        {
            Cargo cargo = await _context.Cargos.FindAsync(id);

            if (cargo == null)
                return null;

            return new Clases.Put.Cargo { Id = cargo.IdCargo, Nombre = cargo.Nombre };
        }
    }
}
