using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Departamento
{
    public int IdDpto { get; set; }

    public int? IdJefeDpto { get; set; }

    public string Nombre { get; set; } = null!;

    public ulong Hab { get; set; }

    public virtual ICollection<Carrera> Carreras { get; set; } = new List<Carrera>();

    public virtual ICollection<Docente> Docentes { get; set; } = new List<Docente>();

    public virtual Docente? IdJefeDptoNavigation { get; set; }
}
