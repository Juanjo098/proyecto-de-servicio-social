using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Put
{
    public class Departamento
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El nombre del departamento no puede quedar vacio")]
        [StringLength(128, ErrorMessage = "El nombre del departamento no puede tener más de 128 caracteres")]
        public string Nombre { get; set; }
    }
}
