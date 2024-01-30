using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Get
{
    public class EstadoGenral
    {

            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = null!;

            [Display(Name = "No. de control")]
            public string NoControl { get; set; } = null!;

            [Display(Name = "SCNI")]
            public ulong Scni { get; set; }

            [Display(Name = "CNI")]
            public ulong Cni { get; set; }

            [Display(Name = "CL")]
            public ulong Cl { get; set; }

            [Display(Name = "CAII")]
            public ulong Caii { get; set; }

            [Display(Name = "RP")]
            public ulong Rp { get; set; }

            [Display(Name = "RPS")]
            public ulong Rps { get; set; }

            [Display(Name = "ST")]
            public ulong St { get; set; }

            [Display(Name = "PRO")]
            public ulong Pro { get; set; }

            [Display(Name = "SL")]
            public ulong Sl { get; set; }

            [Display(Name = "LP")]
            public ulong Lp { get; set; }

            [Display(Name = "AS")]
            public ulong Asnc { get; set; }

            [Display(Name = "OI")]
            public ulong Oi { get; set; }

            [Display(Name = "CURP")]
            public ulong Curp { get; set; }

            [Display(Name = "RFC")]
            public ulong Rfc { get; set; }

            [Display(Name = "CB")]
            public ulong Cb { get; set; }

            [Display(Name = "Esado del proceso")]
            public ulong Estado { get; set; }

            [Display(Name = "Fecha de incio")]
            public DateTime? FechaInicio { get; set; }

            [Display(Name = "Fecha de vencimiento")]
            public DateTime? FechaVencimiento { get; set; }
            [Display(Name = "Fecha de titulacion")]
            public DateTime? FechaTitulacion { get; set; }
    }
}
