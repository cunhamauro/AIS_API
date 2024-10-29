using QRCoder;

namespace AIS_API_Mobile.Helpers
{
    public class QrCodeGenerator : IQrCodeGenerator
    {
        public MemoryStream GenerateQrCode(string qrContent)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    using (var qrCodeBitmap = qrCode.GetGraphic(20))
                    {
                        var memoryStream = new MemoryStream();
                        qrCodeBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        memoryStream.Position = 0;
                        return memoryStream;
                    }
                }
            }
        }
    }
}
