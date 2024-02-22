using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class ReporteDocente
    {
        public int IdDocente { get; set; }
        [Display(Name = "Desde")]
        public string desde { get; set; }
        [Display(Name = "Hasta")]
        public string hasta { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Cedula")]
        public string? Cedula { get; set; }
        [Display(Name = "Departamento")]
        public int IdCarrera { get; set; }

        [Display(Name = "Departamento")]
        public int IdDpto { get; set; }

        [Display(Name = "Veces como presidente")]
        public int VecesPrecidente { get; set; }

        [Display(Name = "Veces como secretario")]
        public int VecesSecretario { get; set; }

        [Display(Name = "Veces como vocal")]
        public int VecesVocal { get; set; }
    }
}
