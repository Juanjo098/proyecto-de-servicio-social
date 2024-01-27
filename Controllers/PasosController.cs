using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Titulacion.Clases.Get;
using Titulacion.Servicios.Contrato;

public class PasosController : Controller
{

    private readonly IBufferedFileUploadService _bufferedFileUploadService;
    private readonly IUsuarioService _usuarioService;

    public PasosController(IBufferedFileUploadService bufferedFileUploadService, IUsuarioService usuarioService)
    {
        _bufferedFileUploadService = bufferedFileUploadService;
        _usuarioService = usuarioService;
    }

    [Authorize(Roles ="3")]
    [Route("/Pasos/CL")]
    public IActionResult SubirSCNI()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Pasos/CL")]
    public async Task<ActionResult> SubirSCNI(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "SCNI";

        int result = await StudentUpload(file, fileSizeLimit, docPrefix);

        switch (result)
        {
            case 1:
                TempData["mensaje"] = "Tu UUID es incorrecto.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "Necesitas subir tu información personal primero antes de subir cualquier archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Tu UUID no se corresponde con su no. de control asignado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "No envió ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 5:
                TempData["mensaje"] = "Excedió el tamaño máximo del archivo: " + fileSizeLimit + "MB.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 6:
                TempData["mensaje"] = "Sólo puede subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "3")]
    [Route("/Pasos/SCNI")]
    public IActionResult SubirCL()
    {
        return View();
    }
    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Pasos/SCNI")]
    public async Task<ActionResult> SubirCL(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "CL";

        int result = await StudentUpload(file, fileSizeLimit, docPrefix);

        switch (result)
        {
            case 1:
                TempData["mensaje"] = "Tu UUID es incorrecto.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "Necesitas subir tu información personal primero antes de subir cualquier archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Tu UUID no se corresponde con su no. de control asignado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "No envió ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 5:
                TempData["mensaje"] = "Excedió el tamaño máximo del archivo: " + fileSizeLimit + "MB.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 6:
                TempData["mensaje"] = "Sólo puede subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> EstadoTitulacion()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // UUID Invalido
        if (!_usuarioService.ValidateGUID(id)){
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "Necesitas subir tu información personal primero antes de subir cualquier archivo.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con su no. de control asignado";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        EstadoProcesoTitulacion data = await _usuarioService.GetEstadoProcesoTitulacion(noControl);

        if (data == null)
        {
            TempData["mensaje"] = "Ocurrió un error con la base de datos";
            TempData["estatus"] = "500";
            return RedirectToAction("CustomError", "Home");
        }

        return View(data);
    }

    // Utilidades
    private async Task<int> StudentUpload(IFormFile file, int fileSizeLimit, string docPrefix)
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id)) return 1;

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null) return 2;

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl)) return 3;

        int result = await _bufferedFileUploadService.UploadFile(file, fileSizeLimit, noControl, docPrefix);
        switch (result)
        {
            // No se envió archivo
            case 1:
                return 4;
            // Se excedio el límite de tamaño del archuivo
            case 2:
                return 5;
            // Extención inválida
            case 3:
                return 6;
        }
        // Todo en orden
        return 0;
    }
}