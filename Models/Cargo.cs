using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Cargo
{
    public int IdCargo { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Hab { get; set; }
}
