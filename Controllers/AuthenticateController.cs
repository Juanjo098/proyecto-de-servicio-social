using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Titulacion.Models;
using Titulacion.Clases.Get;
using Titulacion.Clases;
using Titulacion.Servicios.Contrato;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Titulacion.Controllers
{
    public class AuthenticateController : Controller
    {
        private readonly TitulacionContext _context;
        private readonly IUsuarioService _usuarioService;

        public AuthenticateController(IUsuarioService usuarioService, TitulacionContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        [Route("/IniciarSesion/")]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [Route("/IniciarSesion/")]
        public async Task<IActionResult> IniciarSesion(Sesion modelo)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            modelo.contrasena = Utilidades.EncriptarClave(modelo.contrasena);

            Models.Usuario user;

            try
            {
                user = await _usuarioService.GetUsuario(modelo);
            }

            catch (InvalidOperationException)
            {
                ViewBag.exeption = true;
                return View();
            }

            if (user == null)
            {
                ViewBag.error = true;
                return View();
            }

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, user.IdTipoUsuario.ToString())
                };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
                );

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles ="1,2")]
        [Route("/Administar/RegistrarUsuario")]
        public async Task<IActionResult> RegistrarUsuario()
        {
            ViewBag.tiposUsuario = await ListaTiposUsuario();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "1,2")]
        [Route("/Administar/RegistrarUsuario")]
        public async Task<IActionResult> RegistrarUsuario(Clases.Post.Usuario modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.tiposUsuario = await ListaTiposUsuario();
                return View();
            }

            try
            {
                Models.Usuario usuario = new Models.Usuario
                {
                    Nombre = modelo.Nombre.ToLower(),
                    Correo = modelo.Correo.ToLower(),
                    Contrasena = Utilidades.EncriptarClave(modelo.Contrasena),
                    IdTipoUsuario = modelo.IdTipoUsuario
                };

                if (await _usuarioService.SaveUsuario(usuario))
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.tiposUsuario = await ListaTiposUsuario();
                return View(modelo);
            }
            catch (InvalidOperationException ex)
            {
                return View("CustomError", "Home");
            }
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        
        // Utilidades
        private async Task<List<SelectListItem>> ListaTiposUsuario()
        {
            List<SelectListItem> lista = await (
                    from tipoUsuario in _context.TipoUsuarios
                    where tipoUsuario.Hab == 1 && tipoUsuario.IdTipoUsuario != 1
                    select new SelectListItem {
                        Text = tipoUsuario.Nombre,
                        Value = tipoUsuario.IdTipoUsuario.ToString()
                    }
                ).ToListAsync();
            
            lista.Insert(0, new SelectListItem { Text = "Selecciona el tipo de usuario", Value = "0" });
            
            return lista;
        }
    }
}
