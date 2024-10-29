namespace AIS_API_Mobile.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string model, string folder);
    }
}
