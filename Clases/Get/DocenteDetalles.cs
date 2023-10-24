using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class DocenteDetalles
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Display(Name = "Titulo")]
        public string Titulo { get; set; }
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }
        [Display(Name = "Departamento")]
        public string Departamento { get; set; }
        [Display(Name = "Cargos")]
        public List<string> Cargos { get; set; }
    }
}
