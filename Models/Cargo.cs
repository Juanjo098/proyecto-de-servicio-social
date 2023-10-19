using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Cargo
{
    public int IdCargo { get; set; }

    public string Nombre { get; set; } = null!;

    public ulong Hab { get; set; }
}
