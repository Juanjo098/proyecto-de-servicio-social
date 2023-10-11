using System;
using System.Collections.Generic;

namespace Titulacion.Models;

public partial class Usuario
{
    public Guid IdUsuario { get; set; }

    public int IdTipoUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public ulong MensajesHab { get; set; }

    public ulong Hab { get; set; }

    public virtual TipoUsuario IdTipoUsuarioNavigation { get; set; } = null!;

    public virtual InfoPersonal? InfoPersonal { get; set; }
}
