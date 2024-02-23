using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using Titulacion.Clases;
using Titulacion.Clases.Get;
using Titulacion.Clases.Put;
using Titulacion.Models;
using Titulacion.Servicios.Contrato;

public class PasosController : Controller
{

    private readonly IBufferedFileUploadService _bufferedFileUploadService;
    private readonly IUsuarioService _usuarioService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly TitulacionContext _context;

    public PasosController(IBufferedFileUploadService bufferedFileUploadService, IUsuarioService usuarioService, IWebHostEnvironment webHostEnvironment, TitulacionContext context)
    {
        _bufferedFileUploadService = bufferedFileUploadService;
        _usuarioService = usuarioService;
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Documentos")]
    public async Task<IActionResult> Documentos()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        if (noControl == null)
        {
            TempData["mensaje"] = "Necesitas subir tu información personal primero antes de subir cualquier archivo.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);

        // Puedes enviar la lista de archivos a la vista
        return View(archivos);
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Documentos/Descargar")]
    public async Task<IActionResult> Descargar(string fileName)
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "Sin información personal subida.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "No puede descargar archivos pertenecientes a otro estudiante.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var ruta = GetFilePath(fileName);

        if (!System.IO.File.Exists(ruta))
        {
            TempData["mensaje"] = "El documento no existe.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string extencion = Path.GetExtension(ruta);

        string tipoContenido = GetContentType(extencion);

        // Devuelve el archivo como resultado de la acción
        return PhysicalFile(ruta, tipoContenido, fileName);
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Documentos/Ver")]
    public async Task<IActionResult> VerArchivo(string fileName)
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "Sin información personal subida.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "No puede descargar archivos pertenecientes a otro estudiante.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var ruta = GetFilePath(fileName);

        if (!System.IO.File.Exists(ruta))
        {
            TempData["mensaje"] = "El documento no existe.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var tipo = GetContentType(Path.GetExtension(ruta));

        return PhysicalFile(ruta, tipo);
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Documentos/Eliminar")]
    public async Task<IActionResult> EliminarArchivo(string fileName)
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "Sin información personal subida.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "No puede eliminar archivos pertenecientes a otro estudiante.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        int indiceGuion = fileName.IndexOf('-');

        // Recupera la parte antes del guion
        string resultado = fileName.Substring(0, indiceGuion);

        try
        {
            ProcesoTitulacion data = await _context.ProcesoTitulacions.FirstOrDefaultAsync(p => p.NoControl == noControl);

            if (data == null)
            {
                TempData["mensaje"] = "No ha comenzado su proceso de titulacion todavia.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }

            switch (resultado)
            {
                case "SCNI":
                    data.Scni = 0;
                    break;
                case "CNI":
                    data.Cni = 0;
                    break;
                case "CL":
                    data.Cl = 0;
                    break;
                case "CAII":
                    data.Caii = 0;
                    break;
                case "RP":
                    data.Scni = 0;
                    break;
                case "RPS":
                    data.Rps = 0;
                    break;
                case "ST":
                    data.St = 0;
                    break;
                case "PRO":
                    data.Pro = 0;
                    break;
                case "EGEL":
                    data.Pro = 0;
                    break;
                case "SL":
                    data.Sl = 0;
                    break;
                case "SA":
                    data.Sl = 0;
                    break;
                case "SS":
                    data.Sl = 0;
                    break;
                case "SC":
                    data.Sl = 0;
                    break;
                case "LP":
                    data.Lp = 0;
                    break;
                case "AA":
                    data.Asnc = 0;
                    break;
                case "AS":
                    data.Asnc = 0;
                    break;
                case "AC":
                    data.Asnc = 0;
                    break;
                case "OI":
                    data.Oi = 0;
                    break;
                case "CURP":
                    data.Curp = 0;
                    break;
                case "CB":
                    data.Cb = 0;
                    break;
                case "RFC":
                    data.Rfc = 0;
                    break;
            }

            await _context.SaveChangesAsync();

            var ruta = GetFilePath(fileName);
            // Verifica si el archivo existe antes de intentar eliminarlo
            if (System.IO.File.Exists(ruta))
            {
                // Elimina el archivo
                System.IO.File.Delete(ruta);

                // Puedes realizar otras acciones después de la eliminación si es necesario

                return Content("Archivo eliminado exitosamente.");
            }
            else
            {
                return Content("El archivo no existe.");
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores en caso de que no se pueda eliminar el archivo
            return Content($"Error al eliminar el archivo: {ex.Message}");
        }
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Estado")]
    public async Task<IActionResult> EstadoTitulacion()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
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

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/SCNI")]
    public async Task<IActionResult> SubirSCNI()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivos = archivos.Where(a => a.StartsWith("SCNI") || a.StartsWith("CNI")).ToArray();


        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/SCNI")]
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

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Paso2/CNI")]
    public async Task<IActionResult> SubirCNI()
    {
        string docPrefix = "CNI";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/CL")]
    public async Task<IActionResult> SubirCL()
    {
        string docPrefix = "CL";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }
    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/CL")]
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

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/CAII")]
    public async Task<IActionResult> SubirCAII()
    {
        string docPrefix = "CAII";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/CAII")]
    public async Task<ActionResult> SubirCAII(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "CAII";

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
    [Route("/Proceso-de-Titulacion/Paso2/RP")]
    public async Task<IActionResult> SubirRP()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivos = archivos.Where(a => a.StartsWith("RP")).ToArray();


        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso2/RP")]
    public async Task<ActionResult> SubirRP(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "RP";

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
    [Route("/Proceso-de-Titulacion/Paso3/ST")]
    public async Task<IActionResult> SubirST()
    {
        string docPrefix = "ST";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso3/ST")]
    public async Task<ActionResult> SubirST(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "ST";

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
    [Route("/Proceso-de-Titulacion/Paso3/PRO")]
    public async Task<IActionResult> SubirPRO()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        var pro = archivos.FirstOrDefault(a => a.StartsWith("PRO"));
        var egel = archivos.FirstOrDefault(a => a.StartsWith("EGEL"));

        if (pro != null)
            ViewBag.archivo = pro;
        else if (egel != null)
            ViewBag.archivo = egel;
        else
        {
            ViewBag.archivo = null;
        }

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso3/PRO")]
    public async Task<ActionResult> SubirPRO(IFormFile file, int option)
    {
        int fileSizeLimit = 5;

        string docPrefix = "";

        switch (option)
        {
            case 1:
                docPrefix = "PRO";
                break;
            case 2:
                docPrefix = "EGEL";
                break;
            default:
                TempData["mensaje"] = "Debe elegir una opción válida.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }

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
    [Route("/Proceso-de-Titulacion/Paso4/Solicitudes")]
    public async Task<IActionResult> Solicitud()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivos = archivos.Where(a => a.StartsWith("SL") || a.StartsWith("SA") || a.StartsWith("SS") || a.StartsWith("SC-")).ToArray();

        return View();
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso4/Asignaciones")]
    public async Task<IActionResult> Asignacion()
    {
        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivos = archivos.Where(a => a.StartsWith("LP") || a.StartsWith("AS") || a.StartsWith("AA") || a.StartsWith("AC")).ToArray();

        return View();
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso5/OI")]
    public async Task<IActionResult> OrdenImpresion()
    {
        string docPrefix = "OI";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso7/CURP")]
    public async Task<IActionResult> SubirCURP()
    {
        string docPrefix = "CURP";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso7/CURP")]
    public async Task<ActionResult> SubirCURP(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "CURP";

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
    [Route("/Proceso-de-Titulacion/Paso7/CB")]
    public async Task<IActionResult> SubirCB()
    {
        string docPrefix = "CB";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso7/CB")]
    public async Task<ActionResult> SubirCB(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "CB";

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
    [Route("/Proceso-de-Titulacion/Paso7/RFC")]
    public async Task<IActionResult> SubirRFC()
    {
        string docPrefix = "RFC";

        Guid id = _usuarioService.ConvertToGUID(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // UUID Invalido
        if (!_usuarioService.ValidateGUID(id))
        {
            TempData["mensaje"] = "Tu UUID es incorrecto.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string noControl = await _usuarioService.GetNoControl(id);

        // Sin información personal suvida
        if (noControl == null)
        {
            TempData["mensaje"] = "No ha subido su información personal todavía.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        // UUID y no. de control no coinciden
        if (!await _usuarioService.Validate(id, noControl))
        {
            TempData["mensaje"] = "Tu UUID no se corresponde con el número de control asignado.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var archivos = GetFileList(noControl);
        ViewBag.archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "3")]
    [Route("/Proceso-de-Titulacion/Paso7/RFC")]
    public async Task<ActionResult> SubirRFC(IFormFile file)
    {
        int fileSizeLimit = 5;
        string docPrefix = "RFC";

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

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso1/CNI")]
    public IActionResult SubirCNIAdmin(string noControl, string nombre)
    {
        string docPrefix = "CNI";

        ViewBag.noControl = noControl;
        ViewBag.nombre = nombre;

        var archivos = GetFileList(noControl);
        var archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));
        ViewBag.archivo = archivo;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso1/CNI")]
    public async Task<ActionResult> SubirCNIAdmin(IFormFile file, string noControl, string nombre)
    {
        int fileSizeLimit = 5;
        string docPrefix = "CNI";

        int result = await _bufferedFileUploadService.UploadFile(file, fileSizeLimit, noControl, docPrefix);
        switch (result)
        {
            case 1:
                TempData["mensaje"] = "No se envio ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "El archivo supera los " + fileSizeLimit + "MB";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Solo se pueden subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "Error inesperado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }

        return RedirectToAction("SubirCNIAdmin", new { noControl = noControl, nombre = nombre });
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso1/RPS")]
    public IActionResult SubirRPS(string noControl, string nombre)
    {
        string docPrefix = "RPS";

        ViewBag.noControl = noControl;
        ViewBag.nombre = nombre;

        var archivos = GetFileList(noControl);
        var archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));
        ViewBag.archivo = archivo;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso1/RPS")]
    public async Task<ActionResult> SubirRPS(IFormFile file, string noControl, string nombre)
    {
        int fileSizeLimit = 5;
        string docPrefix = "RPS";

        int result = await _bufferedFileUploadService.UploadFile(file, fileSizeLimit, noControl, docPrefix);
        switch (result)
        {
            case 1:
                TempData["mensaje"] = "No se envio ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "El archivo supera los " + fileSizeLimit + "MB";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Solo se pueden subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "Error inesperado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }

        return RedirectToAction("SubirRPS", new { noControl = noControl, nombre = nombre });
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso4/SL")]
    public IActionResult SubirSL(string noControl, string nombre)
    {
        ViewBag.noControl = noControl;
        ViewBag.nombre = nombre;
        ViewBag.solicitudes = Solicitudes();

        var archivos = GetFileList(noControl);
        ViewBag.archivos = archivos.Where(a => a.StartsWith("SL") || a.StartsWith("SA") || a.StartsWith("SS") || a.StartsWith("SC-")).ToArray();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso4/SL")]
    public async Task<ActionResult> SubirSL(IFormFile file, string noControl, string nombre, int solicitud)
    {
        int fileSizeLimit = 5;
        string docPrefix = "";

        switch (solicitud)
        {
            case 1:
                docPrefix = "SL";
                break;
            case 2:
                docPrefix = "SA";
                break;
            case 3:
                docPrefix = "SS";
                break;
            case 4:
                docPrefix = "SC";
                break;
        }

        int result = await _bufferedFileUploadService.UploadFile(file, fileSizeLimit, noControl, docPrefix);

        switch (result)
        {
            case 1:
                TempData["mensaje"] = "No se envio ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "El archivo supera los " + fileSizeLimit + "MB";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Solo se pueden subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "Error inesperado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }

        return RedirectToAction("SubirSL", new { noControl = noControl, nombre = nombre });
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso4/AS")]
    public IActionResult SubirAS(string noControl, string nombre)
    {
        ViewBag.noControl = noControl;
        ViewBag.nombre = nombre;
        ViewBag.asignaciones = Asignaciones();

        var archivos = GetFileList(noControl);
        ViewBag.archivos = archivos.Where(a => a.StartsWith("LP") || a.StartsWith("AA") || a.StartsWith("AS") || a.StartsWith("AC")).ToArray();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso4/AS")]
    public async Task<ActionResult> SubirAS(IFormFile file, string noControl, string nombre, int asignacion)
    {
        int fileSizeLimit = 5;
        string docPrefix = "";

        switch (asignacion)
        {
            case 1:
                docPrefix = "LP";
                break;
            case 2:
                docPrefix = "AA";
                break;
            case 3:
                docPrefix = "AS";
                break;
            case 4:
                docPrefix = "AC";
                break;
        }

        int result = await _bufferedFileUploadService.UploadFile(file, fileSizeLimit, noControl, docPrefix);

        switch (result)
        {
            case 1:
                TempData["mensaje"] = "No se envio ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "El archivo supera los " + fileSizeLimit + "MB";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Solo se pueden subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "Error inesperado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }

        return RedirectToAction("SubirAS", new { noControl = noControl, nombre = nombre });
    }

    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso5/OI")]
    public IActionResult SubirOI(string noControl, string nombre)
    {
        string docPrefix = "OI";

        ViewBag.noControl = noControl;
        ViewBag.nombre = nombre;

        var archivos = GetFileList(noControl);
        var archivo = archivos.FirstOrDefault(a => a.StartsWith(docPrefix));
        ViewBag.archivo = archivo;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/Administracion/Paso5/OI")]
    public async Task<ActionResult> SubirOI(IFormFile file, string noControl, string nombre)
    {
        int fileSizeLimit = 5;
        string docPrefix = "OI";

        int result = await _bufferedFileUploadService.UploadFile(file, fileSizeLimit, noControl, docPrefix);
        switch (result)
        {
            case 1:
                TempData["mensaje"] = "No se envio ningun archivo.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 2:
                TempData["mensaje"] = "El archivo supera los " + fileSizeLimit + "MB";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 3:
                TempData["mensaje"] = "Solo se pueden subir archivos .pdf, .rar y .zip";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            case 4:
                TempData["mensaje"] = "Error inesperado";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
        }

        return RedirectToAction("SubirOI", new { noControl = noControl, nombre = nombre });
    }


    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/VistaGeneral")]
    public async Task<IActionResult> VistaGeneral(int? numPag, string buscar, int? paso, int? estado, int? carrera, int? departamento)
    {
        int cantidad = 10;
        try
        {
            List<EstadoGenral> items = await ListaAlumnos();

            ViewBag.pasos = Pasos();
            ViewBag.estados = Estados();
            ViewBag.carreras = await Carreras();
            ViewBag.departamentos = await Departamentos();

            if (!string.IsNullOrEmpty(buscar) && items != null)
            {
                items = items.FindAll(item => item.Nombre.ToLower().Contains(buscar.ToLower()));
                ViewBag.buscar = buscar;
            }

            if (paso == null) paso = 0;

            ViewBag.paso = paso;

            switch (paso)
            {
                case 1:
                    items = items.FindAll(item => item.Sl == 1 && item.Lp == 0 && item.Asnc == 0 && item.Oi == 0);
                    break;
                case 2:
                    items = items.FindAll(item => item.Oi == 3);
                    break;
                case 3:
                    items = items.FindAll(item => item.FechaTitulacion != null);
                    break;
            }

            if (estado == null) estado = 0;

            ViewBag.estado = estado;

            if (estado < 4)
            {
                items = items.FindAll(item => item.Estado == Convert.ToUInt64(estado));
            }

            if (carrera == null) carrera = 0;

            ViewBag.carrera = carrera;

            if (carrera > 0)
            {
                items = items.FindAll(item => item.idCarrera == carrera);
            }

            if (departamento == null) departamento = 0;

            ViewBag.departamento = departamento;

            if (departamento > 0)
            {
                items = items.FindAll(item => item.idDpto == departamento);
            }

            var pag = Paginacion<EstadoGenral>.CrearLista(items, numPag ?? 1, cantidad);

            return View(pag);
        }
        catch (InvalidOperationException ex)
        {
            return RedirectToAction("CustomError", "Home");
        }
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Documentos")]
    public IActionResult GestionarDocumentos(string noControl, string nombre)
    {
        var archivos = GetFileList(noControl);
        ViewBag.nombre = nombre;
        ViewBag.noControl = noControl;

        // Puedes enviar la lista de archivos a la vista
        return View(archivos);
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Gestionar-Proceso")]
    public async Task<IActionResult> GestionarProcesoTitulacion(string noControl, string nombre)
    {
        var proc = await GetProcTitulacion(noControl);

        if (proc == null)
        {
            TempData["mensaje"] = "No se encontró al alumno.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        ViewBag.opciones = EstadosDeDocumentos();

        ViewBag.nombre = nombre;

        return View(proc);
    }

    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/Administracion/Gestionar-Titulacion")]
    public async Task<IActionResult> GestionarTitulacion(string noControl)
    {
        var proc = await GetInfoTitulacion(noControl);

        if (proc == null)
        {
            TempData["mensaje"] = "No se encontró al alumno.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        ViewBag.opciones = await GetOpcionesTitulacion();
        ViewBag.docentes = await ListaDocentesNombre();
        ViewBag.alternativas = await Alternativas();
        var estados = Estados();
        estados.RemoveAt(estados.Count - 1);
        ViewBag.estados = estados;

        return View(proc);
    }

    [HttpPost]
    [Authorize(Roles = "1")]
    [Route("/Proceso-de-Titulacion/Administracion/Gestionar-Titulacion")]
    public async Task<IActionResult> GestionarTitulacion(ActualizarInfoTitulacion model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.opciones = await GetOpcionesTitulacion();
            ViewBag.docentes = await ListaDocentesNombre();
            ViewBag.alternativas = await Alternativas();
            ViewBag.estados = Estados();

            return View(model);
        }

        string[] sinodales = { model.Presidente, model.Secretario, model.Vocal, model.Suplente };

        bool distintos = sinodales.Distinct().Count() == sinodales.Length;

        if (!distintos)
        {
            ViewBag.opciones = await GetOpcionesTitulacion();
            ViewBag.docentes = await ListaDocentesNombre();
            ViewBag.alternativas = await Alternativas();
            ViewBag.estados = Estados();

            return View(model);
        }

        var docentes = await ListaDocentes();

        var presidente = GetDonceteNombre(model.Presidente, docentes);
        var secretario = GetDonceteNombre(model.Secretario, docentes);
        var vocal = GetDonceteNombre(model.Vocal, docentes);
        var suplente = GetDonceteNombre(model.Suplente, docentes);

        InformacionTitulacion info = await _context.InformacionTitulacions.FirstOrDefaultAsync(inf => inf.NoControl == model.NoControl);

        DateOnly? fechaCNI = model.FechaCni != null ? DateOnly.Parse(model.FechaCni) : null;
        DateOnly? fechaST = model.FechaSt != null ? DateOnly.Parse(model.FechaSt) : null;
        DateOnly? fechaAarp = model.FechaAarp != null ? DateOnly.Parse(model.FechaAarp) : null;
        DateOnly? fechaArp = model.FechaArp != null ? DateOnly.Parse(model.FechaArp) : null;
        DateOnly? fechaVencimiento = model.FechaVecimiento != null ? DateOnly.Parse(model.FechaVecimiento) : null;
        TimeOnly? horaArp = model.HoraArp != null ? TimeOnly.Parse(model.HoraArp + ":00") : null;

        info.FechaCni = fechaCNI;
        info.FechaSt = fechaST;
        info.FechaAarp = fechaAarp;
        info.FechaArp = fechaArp;
        info.FechaVecimiento = fechaVencimiento;
        info.HoraArp = horaArp;
        info.Producto = model.Producto;
        info.Alternativa = model.Alternativa;
        info.Proyecto = model.Proyecto;
        info.Presidente = presidente?.IdDocente;
        info.Secretario = secretario?.IdDocente;
        info.Vocal = vocal?.IdDocente;
        info.Suplente = suplente?.IdDocente;
        info.Estado = model.Estado;

        await _context.SaveChangesAsync();

        return RedirectToAction("GestionarTitulacion", new { noControl = model.NoControl , nombre  = model.Nombre});
    }

    [HttpPost]
    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Gestionar-Proceso")]
    public async Task<IActionResult> GestionarProcesoTitulacion(ActualizarProcTitulacion model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.opciones = EstadosDeDocumentos();
            return View(model);
        }

        try
        {
            ProcesoTitulacion info = await _context.ProcesoTitulacions.FirstOrDefaultAsync(proc => proc.NoControl == model.NoControl);

            if (info == null)
            {
                TempData["mensaje"] = "No se encontró al alumno.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }

            info.Scni = model.Scni;
            info.Cni = model.Cni;
            info.Cl = model.Cl;
            info.Caii = model.Caii;
            info.Rp = model.Rp;
            info.Rps = model.Rps;
            info.St = model.St;
            info.Pro = model.Pro;
            info.Sl = model.Sl;
            info.Lp = model.Lp;
            info.Asnc = model.Asnc;
            info.Lp = model.Lp;
            info.Oi = model.Oi;
            info.Curp = model.Curp;
            info.Cb = model.Cb;
            info.Rfc = model.Rfc;

            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            TempData["mensaje"] = "Error en la base de datos.";
            TempData["estatus"] = "500";
            return RedirectToAction("CustomError", "Home");
        }

        return RedirectToAction("GestionarDocumentos", "Pasos", new { noControl = model.NoControl, nombre = model.Nombre });

    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Documentos/Descargar")]
    public IActionResult DescargarAdmin(string fileName)
    {
        var ruta = GetFilePath(fileName);

        if (!System.IO.File.Exists(ruta))
        {
            TempData["mensaje"] = "El documento no existe.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        string extencion = Path.GetExtension(ruta);

        string tipoContenido = GetContentType(extencion);

        // Devuelve el archivo como resultado de la acción
        return PhysicalFile(ruta, tipoContenido, fileName);
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Documentos/Ver")]
    public IActionResult VerArchivoAdmin(string fileName)
    {
        var ruta = GetFilePath(fileName);

        if (!System.IO.File.Exists(ruta))
        {
            TempData["mensaje"] = "El documento no existe.";
            TempData["estatus"] = "400";
            return RedirectToAction("CustomError", "Home");
        }

        var tipo = GetContentType(Path.GetExtension(ruta));

        return PhysicalFile(ruta, tipo);
    }

    [Authorize(Roles = "1,2")]
    [Route("/Proceso-de-Titulacion/Administracion/Documentos/Eliminar")]
    public async Task<IActionResult> EliminarArchivoAdmin(string fileName)
    {

        int indiceGuion = fileName.IndexOf('-');

        // Recupera la parte antes del guion
        string resultado = fileName.Substring(0, indiceGuion);

        string noControl = GetNoControlFromFileName(fileName);

        try
        {
            ProcesoTitulacion data = await _context.ProcesoTitulacions.FirstOrDefaultAsync(p => p.NoControl == noControl);

            if (data == null)
            {
                TempData["mensaje"] = "No ha comenzado su proceso de titulacion todavia.";
                TempData["estatus"] = "400";
                return RedirectToAction("CustomError", "Home");
            }

            switch (resultado)
            {
                case "SCNI":
                    data.Scni = 0;
                    break;
                case "CNI":
                    data.Cni = 0;
                    break;
                case "CL":
                    data.Cl = 0;
                    break;
                case "CAII":
                    data.Caii = 0;
                    break;
                case "RP":
                    data.Scni = 0;
                    break;
                case "RPS":
                    data.Rps = 0;
                    break;
                case "ST":
                    data.St = 0;
                    break;
                case "PRO":
                    data.Pro = 0;
                    break;
                case "EGEL":
                    data.Pro = 0;
                    break;
                case "SL":
                    data.Sl = 0;
                    break;
                case "SA":
                    data.Sl = 0;
                    break;
                case "SS":
                    data.Sl = 0;
                    break;
                case "SC":
                    data.Sl = 0;
                    break;
                case "LP":
                    data.Lp = 0;
                    break;
                case "AA":
                    data.Asnc = 0;
                    break;
                case "AS":
                    data.Asnc = 0;
                    break;
                case "AC":
                    data.Asnc = 0;
                    break;
                case "OI":
                    data.Oi = 0;
                    break;
                case "CURP":
                    data.Curp = 0;
                    break;
                case "CB":
                    data.Cb = 0;
                    break;
                case "RFC":
                    data.Rfc = 0;
                    break;
            }

            await _context.SaveChangesAsync();

            var ruta = GetFilePath(fileName);
            // Verifica si el archivo existe antes de intentar eliminarlo
            if (System.IO.File.Exists(ruta))
            {
                // Elimina el archivo
                System.IO.File.Delete(ruta);

                // Puedes realizar otras acciones después de la eliminación si es necesario

                return Content("Archivo eliminado exitosamente.");
            }
            else
            {
                return Content("El archivo no existe.");
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores en caso de que no se pueda eliminar el archivo
            return Content($"Error al eliminar el archivo: {ex.Message}");
        }
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

    private string[] GetFileList(string noControl)
    {
        var rutaCarpeta = Path.Combine(_webHostEnvironment.ContentRootPath, "Expedientes\\" + noControl);

        // Obtén la lista de archivos en la carpeta
        var archivos = Directory.GetFiles(rutaCarpeta);

        return archivos.Select(ruta => Path.GetFileName(ruta)).ToArray();
    }

    private string GetFilePath(string fileName)
    {
        string noControl = GetNoControlFromFileName(fileName);

        return Path.Combine(_webHostEnvironment.ContentRootPath, "Expedientes\\" + noControl + "\\" + fileName);
    }

    private string GetNoControlFromFileName(string fileName)
    {
        int indiceGuion = fileName.IndexOf('-');
        int indicePunto = fileName.IndexOf('.');

        return fileName.Substring(indiceGuion + 1, indicePunto - indiceGuion - 1);
    }

    private string GetContentType(string extencion)
    {
        switch (extencion)
        {
            case ".pdf":
                return "application/pdf";
            case ".rar":
                return "application/x-rar-compressed";

            case ".zip":
                return "application/zip";
        }
        return null;
    }

    private async Task<List<EstadoGenral>> ListaAlumnos()
    {
        try
        {
            return await (
                from alu in _context.InfoPersonals
                join info in _context.InformacionTitulacions
                on alu.NoControl equals info.NoControl
                join proc in _context.ProcesoTitulacions
                on alu.NoControl equals proc.NoControl
                join car in _context.Carreras
                on alu.IdCarrera equals car.IdCarrera
                join dep in _context.Departamentos
                on car.IdDpto equals dep.IdDpto

                where alu.Hab == 1 && info.Hab == 1 && proc.Hab == 1 && car.Hab == 1 && dep.Hab == 1
                select new EstadoGenral
                {
                    idCarrera = car.IdCarrera,
                    idDpto = dep.IdDpto,
                    NoControl = alu.NoControl,
                    Nombre = alu.ApPaterno + " " + alu.ApMaterno + " " + alu.Nombre,
                    Scni = proc.Scni,
                    Cni = proc.Cni,
                    Cl = proc.Cl,
                    Caii = proc.Caii,
                    Rp = proc.Rp,
                    Rps = proc.Rps,
                    St = proc.St,
                    Pro = proc.Pro,
                    Sl = proc.Sl,
                    Lp = proc.Lp,
                    Asnc = proc.Asnc,
                    Oi = proc.Oi,
                    Curp = proc.Curp,
                    Cb = proc.Cb,
                    Rfc = proc.Rfc,
                    Estado = info.Estado,
                    FechaInicio = info.FechaCni,
                    FechaVencimiento = info.FechaVecimiento,
                    FechaTitulacion = info.FechaAarp,
                    Hora = info.HoraArp.ToString(),
                }
                ).ToListAsync();
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private List<SelectListItem> Pasos()
    {
        return new List<SelectListItem> {
            new SelectListItem{ Text = "Todos", Value = "0" },
            new SelectListItem{ Text = "Con solicitud", Value = "1" },
            new SelectListItem{ Text = "Con orden de impresion", Value = "2" },
            new SelectListItem{ Text = "Con fecha de titulacion", Value = "3" },
        };
    }

    private List<SelectListItem> Estados()
    {
        return new List<SelectListItem> {
            new SelectListItem{ Text = "En proceso", Value = "0" },
            new SelectListItem{ Text = "Titulado", Value = "1" },
            new SelectListItem{ Text = "Cancelado", Value = "2" },
            new SelectListItem{ Text = "Reprogramado", Value = "3" },
            new SelectListItem{ Text = "Todos", Value = "4" },

        };
    }

    private async Task<string[]> NombresAlumnos()
    {
        var alumnos = await ListaAlumnos();

        if (alumnos == null) return null;

        var alumnosEnProceso = alumnos.FindAll(alu => alu.Estado != 2).ToList();

        return alumnosEnProceso.Select(alu => alu.Nombre).ToArray();
    }

    private async Task<string> GetNoControlFromName(string name)
    {
        try
        {
            var info = await _context.InfoPersonals.FirstOrDefaultAsync(alu => name == (alu.ApPaterno + " " + alu.ApMaterno + " " + alu.Nombre));

            if (info == null) return null;

            return info.NoControl;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async Task<ActualizarProcTitulacion> GetProcTitulacion(string noControl)
    {
        try
        {
            var data = await _context.ProcesoTitulacions.FirstOrDefaultAsync(p => p.NoControl == noControl);

            if (data == null) return null;

            return new ActualizarProcTitulacion
            {
                NoControl = data.NoControl,
                Scni = data.Scni,
                Cni = data.Cni,
                Cl = data.Cl,
                Caii = data.Caii,
                Rp = data.Rp,
                Rps = data.Rps,
                St = data.St,
                Pro = data.Pro,
                Sl = data.Sl,
                Lp = data.Lp,
                Asnc = data.Asnc,
                Oi = data.Oi,
                Curp = data.Curp,
                Cb = data.Cb,
                Rfc = data.Rfc,
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private List<SelectListItem> EstadosDeDocumentos()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value="0", Text="Pendiente" },
            new SelectListItem { Value="1", Text="Enviado" },
            new SelectListItem { Value="2", Text="Necesita correciones" },
            new SelectListItem { Value="3", Text="Aprobado" },
        };
    }

    private async Task<string[]> ListaDocentesNombre()
    {
        var docentes = await (
                from doc in _context.Docentes
                where doc.Hab == 1
                select doc).ToListAsync();
        var nombres = docentes.Select(docentes => docentes.Nombre).ToArray();

        return nombres;
    }

    private Titulacion.Models.Docente GetDocente(List<Titulacion.Models.Docente> docentes, int? id)
    {
        if (id == null) return null;
        var docente = docentes.FirstOrDefault(d => d.IdDocente == id);
        if (docente == null) return null;
        return docente;
    }

    private async Task<ActualizarInfoTitulacion> GetInfoTitulacion(string noControl)
    {
        try
        {
            var info = await _context.InformacionTitulacions.FirstOrDefaultAsync(i => i.NoControl == noControl);

            var infoPer = await _context.InfoPersonals.FirstOrDefaultAsync(p => p.NoControl == noControl);

            var docentes = await (
                from doc in _context.Docentes
                where doc.Hab == 1
                select doc
                ).ToListAsync();

            var precidente = GetDocente(docentes, info.Presidente);
            var secretario = GetDocente(docentes, info.Secretario);
            var vocal = GetDocente(docentes, info.Vocal);
            var suplente = GetDocente(docentes, info.Suplente);

            if (info == null) return null;

            return new ActualizarInfoTitulacion
            {
                NoControl = info.NoControl,
                Alternativa = info.Alternativa,
                Estado = info.Estado,
                FechaAarp = info.FechaAarp != null ? DateTime.ParseExact(info.FechaAarp.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd") : null,
                FechaArp = info.FechaArp != null ? DateTime.ParseExact(info.FechaArp.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd") : null,
                FechaCni = info.FechaCni != null ? DateTime.ParseExact(info.FechaCni.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd") : null,
                FechaSt = info.FechaSt != null ? DateTime.ParseExact(info.FechaSt.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd") : null,
                FechaVecimiento = info.FechaVecimiento != null ? DateTime.ParseExact(info.FechaVecimiento.ToString(), "dd/MM/yyyy", null).ToString("yyyy-MM-dd") : null,
                HoraArp = info.HoraArp != null ? DateTime.Now.Date.Add(info.HoraArp.Value.ToTimeSpan()).ToString("HH:mm") : null,
                Nombre = infoPer.ApPaterno + " " + infoPer.ApMaterno + " " + infoPer.Nombre,
                Presidente = precidente?.Nombre,
                Secretario = secretario?.Nombre,
                Vocal = vocal?.Nombre,
                Suplente = suplente?.Nombre,
                PresidenteCedula = precidente?.Cedula,
                SecretarioCedula = secretario?.Cedula,
                VocalCedula = vocal?.Cedula,
                Producto = info.Producto,
                Proyecto = info.Proyecto,
                Telefono = infoPer.Telefono
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async Task<string[]> GetOpcionesTitulacion()
    {
        try
        {
            var productos = await (
                    from prods in _context.Productos
                    where prods.Hab == 1
                    select prods
                ).ToListAsync();

            var prodNames = productos?.Select(prod => prod.Producto1);

            var opciones = await (
                from ops in _context.Opciones
                where ops.Hab == 1
                select ops
                ).ToListAsync();

            var opNames = opciones?.Select(prod => prod.Opcion);

            return opNames?.Concat(prodNames).ToArray();

        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<string[]> Alternativas()
    {
        try
        {
            var alternativas = await (
            from alt in _context.Alternativas
            where alt.Hab == 1
            select alt
            ).ToListAsync();

            if (alternativas == null) return null;

            return alternativas.Select(alt => alt.Alternativa1).ToArray();

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async  Task<List<SelectListItem>> Carreras()
    {
        try
        {
            List<SelectListItem> lista = await (
                    from car in _context.Carreras
                    where car.Hab == 1
                    select new SelectListItem { Text = car.Nombre, Value = car.IdCarrera.ToString() }
                ).ToListAsync();

            lista.Insert(0, new SelectListItem { Text = "Todas", Value = "0" }) ;

            return lista;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async Task<List<SelectListItem>> Departamentos()
    {
        try
        {
            List<SelectListItem> lista = await (
                    from dep in _context.Departamentos
                    where dep.Hab == 1
                    select new SelectListItem { Text = dep.Nombre, Value = dep.IdDpto.ToString() }
                ).ToListAsync();

            lista.Insert(0, new SelectListItem { Text = "Todas", Value = "0" });

            return lista;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private bool DocenteExist(string docente, List<Titulacion.Models.Docente> docentes)
    {
        return docentes.FirstOrDefault(doc => doc.Nombre == docente) != null;
    }

    private async Task<bool> ExisteAlternativa(string alternativa)
    {
        try
        {
            return await _context.Alternativas.FirstOrDefaultAsync(alt => alt.Alternativa1 == alternativa) != null;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private Titulacion.Models.Docente GetDonceteNombre(string nombre, List<Titulacion.Models.Docente> docentes)
    {
        return docentes?.FirstOrDefault(doc => doc.Nombre == nombre);
    }

    private async Task<List<Titulacion.Models.Docente>> ListaDocentes()
    {
        try
        {
            var docentes = await (
                    from doc in _context.Docentes
                    where doc.Hab == 1
                    select doc
                ).ToListAsync();
            return docentes;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private List<SelectListItem> Solicitudes ()
    {
        return new List<SelectListItem> {
            new SelectListItem { Text = "Solicitud de liberacion", Value = "1" },
            new SelectListItem { Text = "Solicitud de asesores", Value = "2" },
            new SelectListItem { Text = "Solicitud de sinodales", Value = "3" },
            new SelectListItem { Text = "Solicitud de comité revisor", Value = "4" }
        };
    }

    private List<SelectListItem> Asignaciones()
    {
        return new List<SelectListItem> {
            new SelectListItem { Text = "Liberacion de proyecto", Value = "1" },
            new SelectListItem { Text = "Asignación de asesores", Value = "2" },
            new SelectListItem { Text = "Asignación de sinodales", Value = "3" },
            new SelectListItem { Text = "Asignación de comité revisor", Value = "4" }
        };
    }
}