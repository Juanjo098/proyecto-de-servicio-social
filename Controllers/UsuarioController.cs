using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;
using Microsoft.AspNetCore.Mvc.Rendering;
using Titulacion.Clases.Get;


namespace Titulacion.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly TitulacionContext _context;

        public UsuarioController(TitulacionContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Usuarios")]
        public async Task<IActionResult> Usuarios(int? numPag, string buscar, int tipoUsuario)
        {
            try
            {
                int cantidad = 10;
                var items = await ListaUsuarios();

                if (!string.IsNullOrEmpty(buscar) && items != null)
                {
                    items = items.FindAll(item => item.Nombre.ToUpper().Contains(buscar.ToUpper()));
                    ViewBag.buscar = buscar;
                }

                if (tipoUsuario != 0)
                {
                    items = items.FindAll(item => item.IdTipoUsuario == tipoUsuario);
                    ViewBag.buscarTipoUsuario = tipoUsuario;
                }

                var pag = Paginacion<Clases.Get.Usuario>.CrearLista(items, numPag ?? 1, cantidad);

                ViewBag.tiposUsuario = await ListaTipoUsuarios();
                return View(pag);
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToAction("Custom", "Home");
            }
        }

        [Authorize(Roles = "1,2")]
        [Route("/Administracion/Usuarios/Detalles")]
        public async Task<IActionResult> Detalles(Guid id)
        {
            try
            {
                var user = await GetUsuario(id);
                return View(user);
            }
            catch (Exception e)
            {
                TempData["mensaje"] = e.Message;
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }
        }

        [Authorize(Roles = "1")]
        [Route("/Administracion/Usuarios/Editar")]
        public async Task<IActionResult> Editar(Guid id)
        {
            try
            {
                ViewBag.tipos = TiposUsuario();
                ViewBag.estados = Estados();
                var user = await GetUsuarioEdit(id);

                if (user != null)
                {
                    user.Contrasena = "";
                }

                return View(user);
            }
            catch (Exception e)
            {
                TempData["mensaje"] = e.Message;
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [Route("/Administracion/Usuarios/Editar")]
        public async Task<IActionResult> Editar(Clases.Put.Usuario model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _context.Usuarios.FindAsync(model.IdUsuario) ?? throw new Exception("Usuario no encontrado");
                
                user.Contrasena = Utilidades.EncriptarClave(model.Contrasena);
                user.Correo = model.Correo;
                user.Hab = model.Hab;
                user.IdTipoUsuario = model.IdTipoUsuario;
                user.MensajesHab = model.MensajesHab;
                user.Nombre = model.Nombre;

                await _context.SaveChangesAsync();

                return RedirectToAction("Usuarios");
            }
            catch (Exception e)
            {
                TempData["mensaje"] = e.Message;
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }
        }
        // Usuarios
        private async Task<List<Clases.Get.Usuario>> ListaUsuarios()
        {
            return await (
                    from usuario in _context.Usuarios
                    join tipoUsuario in _context.TipoUsuarios
                    on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                    select new Clases.Get.Usuario {
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        TipoUsuario = tipoUsuario.Nombre,
                        Estado = usuario.Hab == 1 ? "Activo" : "Inactivo",
                        IdTipoUsuario = tipoUsuario.IdTipoUsuario,
                        IdUsuario = usuario.IdUsuario.ToString()
                    }
                ).ToListAsync();
        }

        private async Task<List<SelectListItem>> ListaTipoUsuarios()
        {
            List<SelectListItem> lista = await (
                    from tipoUsuario in _context.TipoUsuarios
                    select new SelectListItem
                    {
                        Text = tipoUsuario.Nombre,
                        Value = tipoUsuario.IdTipoUsuario.ToString()
                    }
                ).ToListAsync();

            lista.Insert(0, new SelectListItem { Text = "Todos los usuarios", Value = "0" });

            return lista;
        }

        private async Task<Clases.Get.Usuario> GetUsuario(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("Error en el UUID");
            }

            var usuario = _context.Usuarios.FirstOrDefault(user => user.IdUsuario == id);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            String tipoUsuario = "";

            if (usuario.IdTipoUsuario == 1)
                tipoUsuario = "Administrador";
            if (usuario.IdTipoUsuario == 2)
                tipoUsuario = "Jefe de depeto";
            if (usuario.IdTipoUsuario == 3)
                tipoUsuario = "Alumno";

            return new Clases.Get.Usuario {
                IdUsuario = usuario.IdUsuario.ToString(),
                Correo = usuario.Correo,
                Estado = usuario.Hab == 1 ? "Activo" : "Inactivo",
                Nombre = usuario.Nombre,
                TipoUsuario = tipoUsuario,
                IdTipoUsuario = usuario.IdTipoUsuario
            };
        }

        private async Task<Clases.Put.Usuario> GetUsuarioEdit(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("Error en el UUID");
            }

            var usuario = _context.Usuarios.FirstOrDefault(user => user.IdUsuario == id);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            return new Clases.Put.Usuario
            {
                IdUsuario = usuario.IdUsuario,
                Correo = usuario.Correo,
                Nombre = usuario.Nombre,
                IdTipoUsuario = usuario.IdTipoUsuario,
                Contrasena = usuario.Contrasena,
                MensajesHab = usuario.MensajesHab,
                Hab = usuario.Hab
            };
        }

        private List<SelectListItem> TiposUsuario() {
            return new List<SelectListItem> {
                new SelectListItem{ Text = "Administrador", Value = "1" },
                new SelectListItem{ Text = "Jefe de departamento", Value = "2" },
                new SelectListItem{ Text = "Alumno", Value = "3" },
            };
        }

        private List<SelectListItem> Estados() {
            return new List<SelectListItem> {
                new SelectListItem{ Text = "Deshabilitado", Value = "1" },
                new SelectListItem{ Text = "Habilitado", Value = "0" },
            };
        }
    }
}
