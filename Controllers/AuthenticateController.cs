using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Titulacion.Models;
using Titulacion.Clases.Get;
using Titulacion.Clases.Post;
using Titulacion.Clases;
using Titulacion.Servicios.Contrato;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Titulacion.Servicios.Implementacion;

namespace Titulacion.Controllers
{
    public class AuthenticateController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AuthenticateController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
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

            Usuario user = await _usuarioService.GetUsuario(modelo);

            if (user == null)
            {
                ViewBag.error = true;
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
        [Route("/Administar/RegistrarUsuario/")]
        public IActionResult RegistrarUsuario()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "1,2")]
        [Route("/Administar/RegistrarUsuario/")]
        public IActionResult RegistrarUsuario(PostUsuario modelo)
        {
            return View();
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
