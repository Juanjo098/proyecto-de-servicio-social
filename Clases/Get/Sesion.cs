using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class Sesion
    {
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "Debes ingresar tu correo")]
        public string correo { get; set; }
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Debes ingresar tu contraseña")]
        public string contrasena { get; set; }
    }
}
