using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class ProcesoTitulacion
{
    public int IdProceso { get; set; }

    public string NoControl { get; set; } = null!;

    public int Paso1 { get; set; }

    public int Scni { get; set; }

    public int Cni { get; set; }

    public int Cl { get; set; }

    public int Caii { get; set; }

    public int Rp { get; set; }

    public int Rps { get; set; }

    public int St { get; set; }

    public int Pro { get; set; }

    public int Sl { get; set; }

    public int Lp { get; set; }

    public int Asnc { get; set; }

    public int Oi { get; set; }

    public int Curp { get; set; }

    public int Rfc { get; set; }

    public int Cb { get; set; }

    public virtual InfoPersonal NoControlNavigation { get; set; } = null!;
}
