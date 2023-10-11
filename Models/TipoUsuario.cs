using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class TipoUsuario
{
    public int IdTipoUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public ulong Hab { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
