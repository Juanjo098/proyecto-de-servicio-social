using Titulacion.Clases.Get;
using Titulacion.Clases.Get;

namespace Titulacion.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Models.Usuario> GetUsuario(Sesion modelo);

        Task<bool> SaveUsuario(Models.Usuario modelo);

        Guid ConvertToGUID(string id);

        bool ValidateGUID(Guid guid);

        Task<bool> InfoExist(Guid id);

        Task<bool> Validate(Guid userId, string noControl);

        Task<string> GetNoControl(Guid userId);

        Task<ProcesoTitulacion> GetProcesoTitulacion(string noControl);
    }
}
