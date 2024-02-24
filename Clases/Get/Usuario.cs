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
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [ScaffoldColumn(false)]
        public int IdTipoUsuario { get; set; }
        [Display(Name = "ID")]
        public string IdUsuario { get; set; }
    }
}
