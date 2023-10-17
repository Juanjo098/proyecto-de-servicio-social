using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class Cargo
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
    }
}
