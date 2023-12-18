using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class UsuarioDetalle
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; }
        [Display(Name = "Correo")]
        public string Correo { get; set; }
        [Display(Name = "Tipo de usuario")]
        public string TipoUsuario { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Apellido paterno")]
        public string ApPaterno { get; set; }
        [Display(Name = "Apellido materno")]
        public string ApMaterno { get; set; }
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }
    }
}
