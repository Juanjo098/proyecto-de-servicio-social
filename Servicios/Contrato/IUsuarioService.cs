using Microsoft.EntityFrameworkCore;
using Titulacion.Models;
using Titulacion.Clases.Get;

namespace Titulacion.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(Sesion modelo);

        Task<bool> SaveUsuario(Usuario modelo);
    }
}
