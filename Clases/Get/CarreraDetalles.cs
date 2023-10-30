using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class CarreraDetalles
    {
        [Display(Name = "ID")]
        public int IdCarrera { get; set; }
        [Display(Name = "Nombre de la carrera")]
        public string Nombre { get; set; }
        [Display(Name = "Departamento")]
        public string Departamento { get; set; }
        [Display(Name = "Jefe de departamento")]
        public string JefeDpto { get; set; }
    }
}
