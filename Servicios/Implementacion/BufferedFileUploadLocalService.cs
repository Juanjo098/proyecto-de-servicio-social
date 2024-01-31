using Microsoft.EntityFrameworkCore;
using Titulacion.Models;
using Titulacion.Servicios.Contrato;

namespace Titulacion.Servicios.Implementacion
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
        private readonly TitulacionContext _context;

        public BufferedFileUploadLocalService(TitulacionContext context)
        {
            _context = context;
        }

        public async Task<int> UploadFile(IFormFile file, int fileMaxSize, string noControl, string prefix)
        {
            string path = "";
            string extention = "";
            try
            {
                if (file == null) return 1;
                if (file.Length > ConvertMegabytesToBytes(fileMaxSize)) return 2;

                extention = Path.GetExtension(file.FileName);

                if (extention != ".pdf" && extention != ".rar" && extention != ".zip") return 3;

                path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Expedientes/" + noControl));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (var fileStream = new FileStream(Path.Combine(path, prefix + "-" + noControl + extention), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                ProcesoTitulacion data = await _context.ProcesoTitulacions.FirstOrDefaultAsync(p => p.NoControl == noControl);

                if (data == null) return 4;

                switch (prefix)
                {
                    case "SCNI":
                        data.Scni = 1;
                        break;
                    case "CNI":
                        data.Cni = 3;
                        break;
                    case "CL":
                        data.Cl = 1;
                        break;
                    case "CAII":
                        data.Caii = 1;
                        break;
                    case "RP":
                        data.Rp = 1;
                        break;
                    case "RPS":
                        data.Rps = 3;
                        break;
                    case "ST":
                        data.St = 1;
                        break;
                    case "PRO":
                        data.Pro = 1;
                        break;
                    case "EGEL":
                        data.Pro = 1;
                        break;
                    case "SL":
                        data.Sl = 1;
                        break;
                    case "SA":
                        data.Sl = 1;
                        break;
                    case "SS":
                        data.Sl = 1;
                        break;
                    case "SC":
                        data.Sl = 1;
                        break;
                    case "LP":
                        data.Lp = 1;
                        data.Sl = 3;
                        break;
                    case "AA":
                        data.Asnc = 1;
                        data.Sl = 3;
                        break;
                    case "AS":
                        data.Asnc = 1;
                        data.Sl = 3;
                        break;
                    case "AC":
                        data.Asnc = 1;
                        data.Sl = 3;
                        break;
                    case "OI":
                        data.Oi = 3;
                        data.Asnc = 3;
                        break;
                    case "CURP":
                        data.Curp = 1;
                        break;
                    case "CB":
                        data.Cb = 1;
                        break;
                    case "RFC":
                        data.Rfc = 1;
                        break;
                }

                await _context.SaveChangesAsync();

                return 0;
            }
            catch (Exception ex)
            {
                return 4;
            }
        }

        private int ConvertMegabytesToBytes (int mb)
        {
            return mb * 1024 * 1024;
        }
    }
}
