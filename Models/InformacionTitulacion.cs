using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class InformacionTitulacion
{
    public int IdInfoTitulacion { get; set; }

    public string NoControl { get; set; } = null!;

    public DateOnly? FechaCni { get; set; }

    public DateOnly? FechaSt { get; set; }

    public DateOnly? FechaAarp { get; set; }

    public DateOnly? FechaArp { get; set; }

    public TimeOnly? HoraArp { get; set; }

    public DateOnly? FechaVecimiento { get; set; }

    public string? Producto { get; set; }

    public string? Alternativa { get; set; }

    public string? Proyecto { get; set; }

    public int? Presidente { get; set; }

    public int? Secretario { get; set; }

    public int? Vocal { get; set; }

    public int? Suplente { get; set; }

    public ulong Hab { get; set; }

    public ulong Estado { get; set; }

    public virtual InfoPersonal NoControlNavigation { get; set; } = null!;
}
