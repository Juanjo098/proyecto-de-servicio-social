using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Put
{
    public class Carrera
    {
        [Display(Name = "ID")]
        public int IdCarrera { get; set; }
        [Display(Name = "Nombre de la carrera")]
        [Required(ErrorMessage = "No puede dejar el campo vacío")]
        [StringLength(128, ErrorMessage = "El campo no debe contener mas de 128 caracteres")]
        public string Nombre { get; set; } = null!;
        [Display(Name = "Departamento")]
        [Range(0, int.MaxValue, ErrorMessage = "Debe elgir un departamento válido.")]
        public int IdDpto { get; set; }
    }
}
