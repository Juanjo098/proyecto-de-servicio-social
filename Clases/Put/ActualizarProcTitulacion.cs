using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Put
{
    public class ActualizarProcTitulacion
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Required]
        [Display(Name = "No. de control")]
        public string NoControl { get; set; } = null!;

        [Display(Name = "SCNI")]
        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        public ulong Scni { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "CNI")]
        public ulong Cni { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "CL")]
        public ulong Cl { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "CAII")]
        public ulong Caii { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "RP")]
        public ulong Rp { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "RPS")]
        public ulong Rps { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "ST")]
        public ulong St { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "PRO")]
        public ulong Pro { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "SL")]
        public ulong Sl { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "LP")]
        public ulong Lp { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "AS")]
        public ulong Asnc { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "OI")]
        public ulong Oi { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "CURP")]
        public ulong Curp { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "RFC")]
        public ulong Rfc { get; set; }

        [Range(0, 3, ErrorMessage = "Debe elegir una opcion valida")]
        [Display(Name = "CB")]
        public ulong Cb { get; set; }
    }
}
