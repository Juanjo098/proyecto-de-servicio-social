using Titulacion.Servicios.Contrato;

namespace Titulacion.Servicios.Implementacion
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
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
