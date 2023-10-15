using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Post
{
    public class Departamento
    {
        [Display(Name = "Nombre del departamento")]
        [Required(ErrorMessage = "No puede dejar este campo en blanco")]
        [StringLength(128, ErrorMessage = "El nombre no puede revasar los 128 caracteres")]
        public string Nombre { get; set; }
    }
}
