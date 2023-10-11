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

        public async Task<Usuario> GetUsuario(Sesion modelo)
        {
            Usuario user = await _context.Usuarios.Where(u =>
                u.Correo == modelo.correo && u.Contrasena == modelo.contrasena
            ).FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> SaveUsuario(Usuario modelo)
        {
            _context.Usuarios.Add(modelo);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
