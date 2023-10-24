using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Post
{
    public class Docente
    {
        [Display(Name = "Departamento")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe eligir el departamento al que pertenece el docente")]
        public int Id_Dpto { get; set; }
        [Display(Name = "Nombre completo")]
        [StringLength(128, ErrorMessage = "El nombre completo debe contener un máximo de 128 caracteres")]
        [Required(ErrorMessage = "No puede dejar este campo en blanco")]
        public string Nombre { get; set; }
        [Display(Name = "Titulo")]
        [StringLength(128, ErrorMessage = "El titulo del docente debe contener un máximo de 128 caracteres")]
        [Required(ErrorMessage = "No puede dejar este campo en blanco")]
        public string Titulo { get; set; }
        [Display(Name = "Diminutivo")]
        [StringLength(128, ErrorMessage = "El diminutivo debe contener un máximo de 16 caracteres")]
        [Required(ErrorMessage = "No puede dejar este campo en blanco")]
        public string Diminutivo { get; set; }
        [Display(Name = "Cédula")]
        [StringLength(128, ErrorMessage = "La cedula debe contener un máximo de 16 caracteres")]
        [Required(ErrorMessage = "No puede dejar este campo en blanco")]
        public string Cedula { get; set; }

    }
}
