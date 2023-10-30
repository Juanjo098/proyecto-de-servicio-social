using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class DepartamentoDetalles
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name = "Nombre del departamento")]
        public string NombreDpto { get; set; }
        [Display(Name = "Jefé de departamento")]
        public string? NombreJefeDpto { get; set; }
        [Display(Name = "Carreras")]
        public List<string> Carreras { get; set; }
    }
}
