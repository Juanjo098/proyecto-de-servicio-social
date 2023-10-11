using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Carrera
{
    public int IdCarrera { get; set; }

    public int IdDpto { get; set; }

    public string Nombre { get; set; } = null!;

    public ulong Hab { get; set; }

    public virtual Departamento IdDptoNavigation { get; set; } = null!;

    public virtual ICollection<InfoPersonal> InfoPersonals { get; set; } = new List<InfoPersonal>();
}
