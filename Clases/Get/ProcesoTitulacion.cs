using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class ProcesoTitulacion
    {
        [Display(Name = "ID del proceo")]
        public int IdProceso { get; set; }

        [Display(Name = "No de control")]
        public string NoControl { get; set; } = null!;

        [Display(Name = "Estado del paso 1")]
        public ulong Paso1 { get; set; }

        [Display(Name = "Solicitud de Constancia de No Inconveniencia")]
        public ulong Scni { get; set; }

        [Display(Name = "Constancia de No Inconveniencia")]
        public ulong Cni { get; set; }

        [Display(Name = "Certificado de Licenciatura")]

        public ulong Cl { get; set; }
        [Display(Name = "Constancia de Acreditación del Idioma Inglés")]

        public ulong Caii { get; set; }

        [Display(Name = "Solicitud de Titulación")]
        public ulong St { get; set; }
        
        [Display(Name = "Proyecto")]
        public ulong Pro { get; set; }

        [Display(Name = "Soliciud de Liberación, Asesores, Sinodales o Comité Revisor")]
        public ulong Sl { get; set; }

        [Display(Name = "Liberación de Proyecto")]
        public ulong Lp { get; set; }

        [Display(Name = "Asignación de Liberación, Asesores, Sinodales o Comité Revisor")]
        public ulong Asnc { get; set; }

        [Display(Name = "Orden de Impresión")]
        public ulong Oi { get; set; }

        [Display(Name = "CURP")]
        public ulong Curp { get; set; }

        [Display(Name = "RFC")]
        public ulong Rfc { get; set; }

        [Display(Name = "Certificado de Bachilleres")]
        public ulong Cb { get; set; }
    }
}
