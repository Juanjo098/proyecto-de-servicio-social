using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class InformacionTitulacion
{
    public int IdInfoTitulacion { get; set; }

    public string NoControl { get; set; } = null!;

    public DateTime? FechaCni { get; set; }

    public DateTime? FechaSt { get; set; }

    public DateTime? FechaAarp { get; set; }

    public DateTime? FechaArp { get; set; }

    public DateTime? FechaVecimiento { get; set; }

    public string? Producto { get; set; }

    public string? Alternativa { get; set; }

    public string? Proyecto { get; set; }

    public int? Presidente { get; set; }

    public int? Secretario { get; set; }

    public int? Vocal { get; set; }

    public int? Suplente { get; set; }

    public ulong Hab { get; set; }

    public virtual InfoPersonal NoControlNavigation { get; set; } = null!;
}
