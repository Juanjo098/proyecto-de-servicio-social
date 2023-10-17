using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Put
{
    public class Cargo
    {
        [Display(Name = "ID")]
        [Required(ErrorMessage = "No puede dejar este campo en blanco")]
        public int Id { get; set; }
        [Display(Name = "Nombre del cargo")]
        [Required(ErrorMessage = "No puede dejar el campo en blanco")]
        [StringLength(128, ErrorMessage = "El nombre del cargo no puede tener más de 128 caracteres")]
        public string Nombre { get; set; }
    }
}
