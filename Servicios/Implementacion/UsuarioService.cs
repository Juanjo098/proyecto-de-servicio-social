using Microsoft.EntityFrameworkCore;
using Titulacion.Clases.Get;
using Titulacion.Models;
using Titulacion.Servicios.Contrato;

namespace Titulacion.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly TitulacionContext _context;

        public UsuarioService(TitulacionContext context)
        {
            _context = context;
        }

        public async Task<Models.Usuario> GetUsuario(Sesion modelo)
        {
            Models.Usuario user = await _context.Usuarios.Where(u =>
                u.Correo == modelo.correo && u.Contrasena == modelo.contrasena
            ).FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> SaveUsuario(Models.Usuario modelo)
        {
            _context.Usuarios.Add(modelo);
            await _context.SaveChangesAsync();

            return true;
        }

        public Guid ConvertToGUID(string id)
        {
            if (String.IsNullOrEmpty(id)) return Guid.Empty;

            if (Guid.TryParse(id, out Guid Guidid))
            {
                return Guidid;
            }

            return Guid.Empty;
        }

        public async Task<bool> InfoExist(Guid id)
        {
            if (id != Guid.Empty)
            {
                try
                {
                    return await _context.InfoPersonals.FirstOrDefaultAsync(info => info.IdUsuario == id) != null;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> Validate(Guid userId, string noControl)
        {
            try
            {
                return await _context.InfoPersonals.FirstOrDefaultAsync(user => user.IdUsuario == userId && user.NoControl == noControl) != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetNoControl(Guid userId)
        {
            try
            {
                InfoPersonal info = await _context.InfoPersonals.FirstOrDefaultAsync(u => u.IdUsuario == userId);
                
                if (info == null) return null;

                return info.NoControl;
            }
            catch (Exception ex) { }
            {
                return null;
            }
        }

        public bool ValidateGUID(Guid guid)
        {
            return (Guid.Empty != guid);
        }
    }
}
