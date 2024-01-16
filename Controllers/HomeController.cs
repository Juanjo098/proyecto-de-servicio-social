using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Titulacion.Clases;
using Titulacion.Models;

namespace Titulacion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Error")]
        public IActionResult CustomError()
        {
            string mensaje = TempData["mensaje"] as string;
            string estatus = TempData["estatus"] as string;
            Mensaje data = new Mensaje { mensaje = mensaje, status = int.Parse(estatus)};
            return View(data);
        }
    }
}