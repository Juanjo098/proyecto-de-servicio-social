using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class DocenteCargo
{
    public int IdDocente { get; set; }

    public int IdCargo { get; set; }

    public virtual Cargo IdCargoNavigation { get; set; } = null!;

    public virtual Docente IdDocenteNavigation { get; set; } = null!;
}
