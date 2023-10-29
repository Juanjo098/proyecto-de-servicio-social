using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Docente
{
    public int IdDocente { get; set; }

    public int IdDpto { get; set; }

    public string Nombre { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string Diminutivo { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public ulong Hab { get; set; }

    public virtual ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();

    public virtual Departamento IdDptoNavigation { get; set; } = null!;
}
