using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class Departamento
    {
        [Display(Name = "ID")]
        public int id { get; set; }
        [Display(Name ="Nombre de pepartamento")]
        public string nombre { get; set; }
    }
}
