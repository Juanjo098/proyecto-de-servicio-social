using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class Usuario
    {
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Correo")]
        public string Correo { get; set; }
        [Display(Name = "Tipo de usuario")]
        public string TipoUsuario { get; set; }
        [Display]
        public string Estado { get; set; }
        public int IdTipoUsuario { get; set; }
        public string IdUsuario { get; set; }
    }
}
