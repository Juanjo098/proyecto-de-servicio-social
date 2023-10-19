using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Docentes()
        {
            return View(await ListaDocentes());
        }

        // Utilidades

        private async Task<List<Clases.Get.Docente>> ListaDocentes() {
            return await (
                    from docente in _context.Docentes
                    where docente.Hab == 1
                    select new Clases.Get.Docente {
                        Id = docente.IdDocente,
                        Nombre = docente.Nombre,
                        Cedula = docente.Cedula
                    }
                ).ToListAsync();
        }
    }
}
