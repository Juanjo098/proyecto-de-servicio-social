using System.ComponentModel.DataAnnotations;

namespace Titulacion.Clases.Put
{
    public class ActualizarInfoTitulacion
    {
        [Display(Name ="No. de control")]
        public string NoControl { get; set; } = null!;

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name ="Telefono")]
        public string Telefono { get; set; }

        [Display(Name ="Fecha CNI")]
        public DateTime? FechaCni { get; set; }

        [Display(Name ="Fecha ST")]
        public DateTime? FechaSt { get; set; }

        [Display(Name ="Fecha AARP")]
        public DateTime? FechaAarp { get; set; }

        [Display(Name ="Fecha ARP")]
        public DateTime? FechaArp { get; set; }

        [Display(Name ="Fecha de Vencimiento")]
        public DateTime? FechaVecimiento { get; set; }

        [Display(Name ="Producto")]
        public string? Producto { get; set; }

        [Display(Name ="Alternativa")]
        public string? Alternativa { get; set; }

        [Display(Name ="Proyecto")]
        public string? Proyecto { get; set; }

        [Display(Name ="Presidente")]
        public string? Presidente { get; set; }

        [Display(Name = "Cedula del presidente")]
        public string? PresidenteCedula { get; set; }

        [Display(Name ="Secretario")]
        public string? Secretario { get; set; }

        [Display(Name = "Cedula del secretario")]
        public string? SecretarioCedula { get; set; }

        [Display(Name ="Vocal")]
        public string? Vocal { get; set; }

        [Display(Name = "Cedula del vocal")]
        public string? VocalCedula { get; set; }

        [Display(Name ="Suplente")]
        public string? Suplente { get; set; }

        [Display(Name ="Estado")]
        public ulong Estado { get; set; }
    }
}
