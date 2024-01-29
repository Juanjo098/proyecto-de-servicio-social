using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class ProcesoTitulacion
{
    public int IdProceso { get; set; }

    public string NoControl { get; set; } = null!;

    public ulong Paso1 { get; set; }

    public ulong Scni { get; set; }

    public ulong Cni { get; set; }

    public ulong Cl { get; set; }

    public ulong Caii { get; set; }

    public ulong Rp { get; set; }

    public ulong Rps { get; set; }

    public ulong St { get; set; }

    public ulong Pro { get; set; }

    public ulong Sl { get; set; }

    public ulong Lp { get; set; }

    public ulong Asnc { get; set; }

    public ulong Oi { get; set; }

    public ulong Curp { get; set; }

    public ulong Rfc { get; set; }

    public ulong Cb { get; set; }

    public ulong Hab { get; set; }

    public virtual InfoPersonal NoControlNavigation { get; set; } = null!;
}
