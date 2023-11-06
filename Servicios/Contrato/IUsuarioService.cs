using Microsoft.EntityFrameworkCore;
using Titulacion.Models;
using Titulacion.Clases.Get;

namespace Titulacion.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Models.Usuario> GetUsuario(Sesion modelo);

        Task<bool> SaveUsuario(Models.Usuario modelo);
    }
}
