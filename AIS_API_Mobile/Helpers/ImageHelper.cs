namespace AIS_API_Mobile.Helpers
{
    public class ImageHelper : IImageHelper
    {
        /// <summary>
        /// Upload an image and save it to a wwwroot specific folder
        /// </summary>
        /// <param name="imageFile">Image file</param>
        /// <param name="text">Text to customize file name</param>
        /// <param name="folder">Folder to save</param>
        /// <returns></returns>
        public async Task<string> UploadImageAsync(IFormFile imageFile, string text, string folder)
        {
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}_{text}.jpg".Replace(" ", "_");

            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/images/{folder}/{file}";
        }
    }
}
