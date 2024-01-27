namespace Titulacion.Servicios.Contrato
{
    public interface IBufferedFileUploadService
    {
        Task<int> UploadFile(IFormFile file, int fileMaxSize, string noControl, string prefix);
    }
}
