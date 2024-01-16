using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Post
{
    public class InformacionPersonal
    {
        [ScaffoldColumn(false)]
        [Required]
        public Guid idUsuario { get; set; }
        [Display(Name = "No. de control")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "Este campo debe tener 8-9 caracteres")]
        [Required(ErrorMessage = "Este campo no puede quedar vacío")]
        public string noControl { get; set; }
        [Display(Name = "Carrera")]
        [Range(1,int.MaxValue, ErrorMessage = "Eliga una carrera")]
        [Required(ErrorMessage = "Este campo no puede quedar vacío")]
        public int idCarrera { get; set; }
        [Display(Name = "Nombre")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "Este campo debe tener 2-64 caracteres")]
        [Required(ErrorMessage = "Este campo no puede quedar vacío")]
        public string nombre { get; set; }
        [Display(Name = "Apellido Materno")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Este campo debe tener 2-32 caracteres")]
        [Required(ErrorMessage = "Este campo no puede quedar vacío")]
        public string apellidoPaterno { get; set; }
        [Display(Name = "Apellido Paterno")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Este campo debe tener 2-32 caracteres")]
        [Required(ErrorMessage = "Este campo no puede quedar vacío")]
        public string apellidoMaterno { get; set; }
        [Display(Name = "Teléfono")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Este campo debe tener 10 caracteres")]
        [Required(ErrorMessage = "Este campo no puede quedar vacío")]
        public string telefono { get; set; }
        [Display(Name = "Dirección")]
        [StringLength(128, ErrorMessage = "Este campo debe tener 128 caracteres máximo")]
        public string? direccion { get; set; }
    }
}
