using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class Docente
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name = "Nombre del docente")]
        public string Nombre { get; set; }
        [Display(Name = "Cedula")]
        public string Cedula { get; set;}
    }
}
