using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Post
{
    public class Usuario
    {
        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        public string Nombre { get; set; }
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        public string Correo { get; set; }
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Este campo no puede quedar vacio")]
        public string Contrasena { get; set; }
        [Display(Name = "Tipo de usuario")]
        [Range(2, 3, ErrorMessage = "Debe elegir un tipo de usuario")]
        public int IdTipoUsuario { get; set; }
    }
}
