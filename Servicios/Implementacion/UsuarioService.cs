﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<EstadoProcesoTitulacion> GetEstadoProcesoTitulacion(string noControl)
        {
            try
            {
                var data = await (
                    from estado in _context.ProcesoTitulacions
                    join infoUsuario in _context.InfoPersonals
                    on estado.NoControl equals infoUsuario.NoControl
                    join inf in _context.InformacionTitulacions
                    on estado.NoControl equals inf.NoControl
                    where inf.Estado == 0
                    select new EstadoProcesoTitulacion
                    {
                        IdProceso = estado.IdProceso,
                        NoControl = estado.NoControl,
                        Paso1 = estado.Paso1,
                        Scni = estado.Scni,
                        Cni = estado.Cni,
                        Cl = estado.Cl,
                        Caii = estado.Caii,
                        Rp = estado.Rp,
                        Rps = estado.Rps,
                        St = estado.St,
                        Pro = estado.Pro,
                        Sl = estado.Sl,
                        Lp = estado.Lp,
                        Asnc = estado.Asnc,
                        Oi = estado.Oi,
                        Cb = estado.Cb,
                        Curp = estado.Curp,
                        Rfc = estado.Rfc,
                    }
                    ).FirstOrDefaultAsync();

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
