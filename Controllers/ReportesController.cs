using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases.Get;
using Titulacion.Models;

namespace Titulacion.Controllers
{
    public class ReportesController : Controller
    {
        private readonly TitulacionContext _context;
        public ReportesController(TitulacionContext context)
        {
                _context = context;
        }

        [Authorize(Roles ="1,2")]
        [Route("/Administracion/Reportes/Individual")]
        public async Task<IActionResult> ReporteIndividual()
        {
            ViewBag.nombres = await GetNombresDocentes();
            DateTime fechaActual = DateTime.Now;

            // Crear una variable de tipo DateOnly para el 1 de enero del año actual
            DateOnly desde = new DateOnly(fechaActual.Year, 1, 1);
            DateOnly hasta = new DateOnly(fechaActual.Year, 12, 31);

            ViewBag.desde = DateTime.ParseExact(desde.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
            ViewBag.hasta = DateTime.ParseExact(hasta.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Reportes/Individual")]
        public async Task<IActionResult> ReporteIndividual(ReporteDocente model)
        {
            try
            {
                if (!ModelState.IsValid) { 
                    ViewBag.nombres = await GetNombresDocentes();
                    ViewBag.desde = model.desde;
                    ViewBag.hasta = model.hasta;
                    return View(model);
                }

                DocenteExtendido docente = await GetDocenteExtendido(model.Nombre);

                if (docente == null) {
                    ViewBag.nombres = await GetNombresDocentes();
                    ViewBag.desde = model.desde;
                    ViewBag.hasta = model.hasta;
                    return View(model);
                }

                DateOnly desde = DateOnly.Parse(model.desde);
                DateOnly hasta = DateOnly.Parse(model.hasta);

                List<InformacionTitulacion> titlaciones = await GetTitulaciones(desde, hasta);

                if (titlaciones == null) return View(null);

                ReporteDocente reporte = new ReporteDocente
                {
                    Nombre = docente.Nombre,
                    Cedula = docente.Cedula,
                    IdCarrera = docente.IdCarrera,
                    IdDpto = docente.IdDpto,
                    VecesPrecidente = 0,
                    VecesSecretario = 0,
                    VecesVocal = 0,
                };

                foreach (InformacionTitulacion titulacion in titlaciones)
                {
                    ContarParticipaciones(docente, titulacion, reporte);
                }

                ViewBag.nombres = await GetNombresDocentes();
                ViewBag.desde = model.desde;
                ViewBag.hasta = model.hasta;
                return View(reporte);
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ex.Message;
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Reportes/Grupal")]
        public async Task<IActionResult> ReporteGrupal(int? carrera, int? departamento, string? nombre, string desde, string hasta)
        {

            if (carrera == null && departamento == null && nombre == null) {
                DateTime fechaActual = DateTime.Now;

                // Crear una variable de tipo DateOnly para el 1 de enero del año actual
                DateOnly inicio = new DateOnly(fechaActual.Year, 1, 1);
                DateOnly final = new DateOnly(fechaActual.Year, 12, 31);

                ViewBag.desde = DateTime.ParseExact(inicio.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                ViewBag.hasta = DateTime.ParseExact(final.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                ViewBag.carreras = await Carreras();
                ViewBag.departamentos = await Departamentos();
                return View();
            }

            try
            {
                DateOnly desdeFecha = DateOnly.Parse(desde);
                DateOnly hastaFecha = DateOnly.Parse(hasta);

                List<InformacionTitulacion> titlaciones = await GetTitulaciones(desdeFecha, hastaFecha);

                List<DocenteExtendido> docentes = await GetDocenteExtendidos();

                ViewBag.desde = desde;
                ViewBag.hasta = hasta;
                ViewBag.carreras = await Carreras();
                ViewBag.departamentos = await Departamentos();

                if (carrera == null) carrera = 0;
                ViewBag.carrera = carrera;

                if (departamento == null) departamento = 0;
                ViewBag.departamento = departamento;

                if (docentes == null) {
                    return View();
                }

                if (titlaciones == null)
                {
                    return View();
                }

                List<ReporteDocente> reportes = new List<ReporteDocente>();

                foreach (DocenteExtendido docente in docentes)
                {
                    ReporteDocente reporte = new ReporteDocente
                    {
                        Nombre = docente.Nombre,
                        Cedula = docente.Cedula,
                        IdDocente = docente.IdDocente,
                        IdDpto = docente.IdDpto,
                        IdCarrera = docente.IdCarrera,
                        VecesPrecidente = 0,
                        VecesSecretario = 0,
                        VecesVocal = 0
                    };

                    foreach (InformacionTitulacion titulacion in titlaciones)
                    {
                        ContarParticipaciones(docente, titulacion, reporte);
                    }

                    reportes.Add(reporte);
                }

                if (departamento > 0)
                    reportes = reportes.FindAll(rep => rep.IdDpto == departamento);

                if (carrera > 0)
                    reportes = reportes.FindAll(rep => rep.IdCarrera == carrera);

                if (nombre != null)
                    reportes = reportes.FindAll(rep => rep.Nombre == nombre);

                return View(reportes);
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ex.Message;
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }
        }

        private async Task<List<InformacionTitulacion>> GetTitulaciones(DateOnly desde, DateOnly hasta)
        {
            try
            {
                List<InformacionTitulacion> titulaciones = await (
                        from titulacion in _context.InformacionTitulacions
                        where titulacion.FechaArp >= desde && titulacion.FechaArp <= hasta
                        select titulacion
                    ).ToListAsync();
                
                return titulaciones;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<DocenteExtendido> GetDocenteExtendido(string nombre)
        {
            try
            {
                return await (
                        from doc in _context.Docentes
                        join dep in _context.Departamentos
                        on doc.IdDpto equals dep.IdDpto
                        join car in _context.Carreras
                        on dep.IdDpto equals car.IdDpto
                        where doc.Hab == 1 && dep.Hab == 1 && car.Hab == 1 && doc.Nombre == nombre
                        select new DocenteExtendido
                        {
                            Nombre = doc.Nombre,
                            Cedula = doc.Cedula,
                            IdDpto = doc.IdDpto,
                            IdDocente = doc.IdDocente,
                            IdCarrera = car.IdCarrera
                        }
                    ).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<string[]> GetNombresDocentes()
        {
            try
            {
                string[] nombres = await _context.Docentes.Select(doc => doc.Nombre).ToArrayAsync();
                return nombres;
            }
            catch (Exception ex) {
            
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<DocenteExtendido>> GetDocenteExtendidos()
        {
            try
            {
                return await (
                       from doc in _context.Docentes
                       join dep in _context.Departamentos
                       on doc.IdDpto equals dep.IdDpto
                       join car in _context.Carreras
                       on dep.IdDpto equals car.IdDpto
                       where doc.Hab == 1 && dep.Hab == 1 && car.Hab == 1
                       select new DocenteExtendido
                       {
                           Nombre = doc.Nombre,
                           Cedula = doc.Cedula,
                           IdDpto = doc.IdDpto,
                           IdDocente = doc.IdDocente,
                           IdCarrera = car.IdCarrera
                       }
                   ).ToListAsync();
            }
            catch (Exception ex) {   
                throw new Exception(ex.Message);
            }
        }

        private void ContarParticipaciones(DocenteExtendido docente, InformacionTitulacion titulacion, ReporteDocente reporte)
        {
            if (titulacion.Presidente == docente.IdDocente)
            {
                reporte.VecesPrecidente++;
            }
            if (titulacion.Secretario == docente.IdDocente)
            {
                reporte.VecesSecretario++;
            }
            if (titulacion.Vocal == docente.IdDocente)
            {
                reporte.VecesVocal++;
            }
        }

        private async Task<List<SelectListItem>> Carreras()
        {
            try
            {
                List<SelectListItem> carreras = await (
                        from car in _context.Carreras
                        where car.Hab == 1
                        select new SelectListItem { Text = car.Nombre, Value = car.IdCarrera.ToString() }
                    ).ToListAsync();
                carreras.Insert(0, new SelectListItem { Text = "Todas", Value = "0" });
                return carreras;
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<SelectListItem>> Departamentos()
        {
            try
            {
                List<SelectListItem> departamentos = await (
                        from dep in _context.Departamentos
                        where dep.Hab == 1
                        select new SelectListItem { Text = dep.Nombre, Value = dep.IdDpto.ToString() }
                    ).ToListAsync();
                departamentos.Insert(0, new SelectListItem { Text = "Todas", Value = "0" });
                return departamentos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
