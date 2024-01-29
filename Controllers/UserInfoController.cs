using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Titulacion.Clases.Post;
using Titulacion.Models;
using Titulacion.Servicios.Contrato;

namespace Titulacion.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly TitulacionContext _context;
        private readonly IUsuarioService _usuarioService;

        public UserInfoController(TitulacionContext context, IUsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
        }

        [Authorize(Roles = "3")]
        [Route("/Alumnos/InformacionPersonal/Insertar")]
        public async Task<IActionResult> Insertar()
        {
            var id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (id == Guid.Empty)
            {
                TempData["mensaje"] = "Error de autenticación. No se reconoce tu userID";
                TempData["estatus"] = "404";
                return RedirectToAction("CustomError", "Home");
            }

            if (!(await _usuarioService.InfoExist(id)))
            {
                ViewBag.carreras = await GetCarreras();
                return View();
            }
            return RedirectToAction("Editar");
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        [Route("/Alumnos/InformacionPersonal/Insertar")]
        public async Task<IActionResult> Insertar(InformacionPersonal model)
        {
            model.idUsuario = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!ModelState.IsValid) {
                ViewBag.carreras = await GetCarreras();
                return View(model);
            }

            try
            {
                if (model.idUsuario != Guid.Empty)
                {
                    InfoPersonal data = new InfoPersonal {
                        NoControl = model.noControl,
                        IdUsuario = model.idUsuario,
                        IdCarrera = model.idCarrera,
                        Nombre = model.nombre,
                        ApPaterno = model.apellidoPaterno,
                        ApMaterno = model.apellidoMaterno,
                        Telefono = model.telefono,
                        Direccion = model.direccion,
                    };

                    ProcesoTitulacion proceso = new ProcesoTitulacion { NoControl = data.NoControl };

                    InformacionTitulacion informacionTitulacion = new InformacionTitulacion { NoControl = data.NoControl };

                    await _context.InfoPersonals.AddAsync(data);
                    await _context.ProcesoTitulacions.AddAsync(proceso);
                    await _context.InformacionTitulacions.AddAsync(informacionTitulacion);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("CustomError", "Error", new { mensaje = "Falló la conexión con la base de datos" });
            }
        }

        [Authorize(Roles = "3")]
        [Route("/Alumnos/InformacionPersonal/Editar")]
        public async Task<IActionResult> Editar()
        {
            var info = await GetInformacionPersonal();
            ViewBag.carreras = await GetCarreras();
            return View(info);
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        [Route("/Alumnos/InformacionPersonal/Editar")]
        public async Task<IActionResult> Editar(InformacionPersonal model)
        {
            var userId = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            model.idUsuario = userId;

            if (model.idUsuario == Guid.Empty)
            {
                TempData["mensaje"] = "Error de autenticación. No se reconoce userID.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }


            if (!ModelState.IsValid)
            {
                ViewBag.carreras = await GetCarreras();
                return View(model);
            }

            try
            {
                if (!(await _usuarioService.InfoExist(model.idUsuario)))
                {
                    TempData["mensaje"] = "No hay información que editar";
                    TempData["estatus"] = "404";
                    return RedirectToAction("CustomError", "Home");
                }

                if (!(await _usuarioService.Validate(model.idUsuario, model.noControl)))
                {
                    TempData["mensaje"] = "No hay información que editar";
                    TempData["estatus"] = "404";
                    return RedirectToAction("CustomError", "Home");
                }

                InfoPersonal data = await _context.InfoPersonals.FindAsync(model.noControl);
                data.IdCarrera = model.idCarrera;
                data.Nombre = model.nombre;
                data.ApPaterno = model.apellidoPaterno;
                data.ApMaterno = model.apellidoMaterno;
                data.Telefono = model.telefono;
                data.Direccion = model.direccion;
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["mensaje"] = "No se pudo conectar a la base de datos.";
                TempData["estatus"] = "500";
                return RedirectToAction("CustomError", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task<List<SelectListItem>> GetCarreras()
        {
            var carreras = await (
                    from carrera in _context.Carreras
                    where carrera.Hab == 1
                    select new SelectListItem { Text = carrera.Nombre, Value = carrera.IdCarrera.ToString()}
                ).ToListAsync();
            carreras.Insert(0, new SelectListItem
            {
                Text = "- Elige tu carrera -",
                Value = "0"
            });
            return carreras;
        }

        private async Task<InformacionPersonal> GetInformacionPersonal()
        {
            var userID = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userID != Guid.Empty)
            {
                var user = await _context.InfoPersonals.FirstOrDefaultAsync(u => u.IdUsuario == userID);
                if (user != null)
                {
                    return new InformacionPersonal
                        {
                            noControl = user.NoControl,
                            idUsuario = userID,
                            idCarrera = user.IdCarrera,
                            nombre = user.Nombre,
                            apellidoPaterno = user.ApPaterno,
                            apellidoMaterno = user.ApMaterno,
                            telefono = user.Telefono,
                            direccion = user.Direccion
                        };
                }
            }
            return null;
        }
    }
}
