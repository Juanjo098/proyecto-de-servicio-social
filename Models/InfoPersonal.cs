using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class InfoPersonal
{
    public string NoControl { get; set; } = null!;

    public Guid IdUsuario { get; set; }

    public int IdCarrera { get; set; }

    public string Nombre { get; set; } = null!;

    public string ApPaterno { get; set; } = null!;

    public string ApMaterno { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string? Direccion { get; set; }

    public ulong Hab { get; set; }

    public virtual Carrera IdCarreraNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
