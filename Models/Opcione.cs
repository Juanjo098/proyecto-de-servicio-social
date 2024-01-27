using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Opcione
{
    public int IdOpcion { get; set; }

    public string Opcion { get; set; } = null!;

    public ulong Hab { get; set; }
}
