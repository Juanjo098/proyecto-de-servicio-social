using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Producto1 { get; set; } = null!;

    public ulong Hab { get; set; }
}
