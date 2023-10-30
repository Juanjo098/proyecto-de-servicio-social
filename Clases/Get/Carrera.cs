using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class Carrera
    {
        [Display(Name = "ID")]
        public int IdCarrera { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;
    }
}
