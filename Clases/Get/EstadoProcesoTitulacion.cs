using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class EstadoProcesoTitulacion
    {
        [Display(Name = "Id del proceso")]
        public int IdProceso { get; set; }

        [Display(Name = "No. de control")]
        public string NoControl { get; set; } = null!;

        [Display(Name = "Estado del paso 1")]
        public int Paso1 { get; set; }

        [Display(Name = "Solicitud de Constancia de No Inconveniencia")]
        public int Scni { get; set; }

        [Display(Name = "Constancia de No Inconveniencia")]
        public int Cni { get; set; }

        [Display(Name = "Certificado de Licenciatura")]
        public int Cl { get; set; }

        [Display(Name = "Constancia de Acreditación del Idioma Inglés")]
        public int Caii { get; set; }

        [Display(Name = "Recibo de Pago")]
        public int Rp { get; set; }

        [Display(Name = "Recibo de Pago Sellado")]
        public int Rps { get; set; }

        [Display(Name = "Solicitud de Titulación")]
        public int St { get; set; }

        [Display(Name = "Proyecto")]
        public int Pro { get; set; }

        [Display(Name = "Solicitud de Liberación, Acesores, Sinodales o Comité Revisor")]
        public int Sl { get; set; }

        [Display(Name = "Liberación de Proyecto")]
        public int Lp { get; set; }

        [Display(Name = "Asignación de Acesores, Sinodales o Comité Revisor")]
        public int Asnc { get; set; }

        [Display(Name = "Orden de Impresión")]
        public int Oi { get; set; }

        [Display(Name = "CURP")]
        public int Curp { get; set; }

        [Display(Name = "RFC")]
        public int Rfc { get; set; }

        [Display(Name = "Certficado de Bachilleres")]
        public int Cb { get; set; }
    }
}
