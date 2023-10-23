using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;

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
            var items = await ListaDocentes();

            if (!string.IsNullOrEmpty(buscar) && items != null)
            {
                items = items.FindAll(item => item.Nombre.ToLower().Contains(buscar.ToLower()));
                ViewBag.buscar = buscar;
            }

            var pag = Paginacion<Clases.Get.Docente>.CrearLista(items, numPag ?? 1, cantidad);
            return View(pag);
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
    }
}
