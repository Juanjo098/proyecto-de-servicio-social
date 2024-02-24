using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Put
{
    public class Usuario
    {
        [Display(Name = "ID")]
        public Guid IdUsuario { get; set; }

        [Display(Name = "Tipo de usuario")]
        [Required(ErrorMessage = "No puede quedar vacio")]
        public int IdTipoUsuario { get; set; }

        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage = "No puede quedar vacio")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "No puede quedar vacio")]
        public string Correo { get; set; } = null!;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "No puede quedar vacio")]
        public string Contrasena { get; set; } = null!;

        [Display(Name = "Habilitar mensajes")]
        [Required(ErrorMessage = "No puede quedar vacio")]
        public ulong MensajesHab { get; set; }
        [Display(Name = "Habilitar Usuario")]
        public ulong Hab { get; set; }
    }
}
