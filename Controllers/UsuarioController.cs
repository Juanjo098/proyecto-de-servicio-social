using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Titulacion.Models;
using Microsoft.EntityFrameworkCore;
using Titulacion.Clases;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<IActionResult> Usuarios(int? numPag, string buscar, int? tipoUsuario)
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

            lista.Insert(0, new SelectListItem { Text = "Elige un tipo de usuario", Value = "0" });

            return lista;
        }
    }
}
