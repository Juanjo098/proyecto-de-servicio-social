using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Alternativa
{
    public int IdAlternativa { get; set; }

    public string Alternativa1 { get; set; } = null!;

    public ulong Hab { get; set; }
}
