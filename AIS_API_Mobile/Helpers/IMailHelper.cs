using AIS_API_Mobile.HelperClasses;

namespace AIS_API_Mobile.Helpers
{
    public interface IMailHelper
    {
        Task<Response> SendEmailAsync(string to, string subject, string body, MemoryStream pdfStream, string pdfName, MemoryStream qrCodeStream);

        string GetHtmlTemplateTokenLink(string title, string subtitle, string button, string tokenLink);

        string GetHtmlTemplateTicket(string title, string nameWithTitle, string idNumber, string flightNumber, string originCityCountry, string destinationCityCountry, string seat, DateTime departure, DateTime arrival, bool cancel);

        string GetHtmlTemplateInvoice(string name, string flightNumber, decimal price);
    }
}
