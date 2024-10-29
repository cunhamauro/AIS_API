namespace AIS_API_Mobile.Helpers
{
    public interface IQrCodeGenerator
    {
        MemoryStream GenerateQrCode(string qrContent);
    }
}
