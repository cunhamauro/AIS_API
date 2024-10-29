using PdfSharp.Drawing;
using PdfSharp.Pdf;
using static AIS_API_Mobile.Helpers.PdfGenerator;

namespace AIS_API_Mobile.Helpers
{
    public class PdfGenerator : IPdfGenerator
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public PdfGenerator(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Generate a PDF for the Flight Ticket
        /// </summary>
        /// <param name="nameWithTitle">Title + Name of Ticket holder</param>
        /// <param name="flightNumber">Flight Number</param>
        /// <param name="originCityCountry">City and Country of Origin</param>
        /// <param name="destinationCityCountry">City and Country of Destination</param>
        /// <param name="seat">Ticket's Seat</param>
        /// <param name="departure">Date of departure</param>
        /// <param name="arrival">Date of arrival</param>
        /// <returns>Ticket PDF MemoryStream</returns>
        public MemoryStream GenerateTicketPdf(string nameWithTitle, string idNumber, string flightNumber, string originCityCountry, string destinationCityCountry, string seat, DateTime departure, DateTime arrival, MemoryStream qrCode)
        {
            // Create a new PDF document
            var document = new PdfDocument();

            // Add a title
            document.Info.Title = $"Ticket for Flight {flightNumber} - {nameWithTitle}";

            // Create an empty page
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Load and draw the AIS logo
            string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "AIS-logo.png"); // To get the path of the AIS logo, relative to wwwroot
            XImage logo = XImage.FromFile(imagePath);
            gfx.DrawImage(logo, 50, 50, 153, 69);

            // Set font
            var fontHeader = new XFont("Verdana", 16, XFontStyleEx.Bold);
            var fontText = new XFont("Verdana", 12, XFontStyleEx.Regular);

            // Calculate the boarding time (30 minutes before departure)
            DateTime boardingTime = departure.AddMinutes(-30);

            // Draw flight information text
            gfx.DrawString($"FLIGHT [{flightNumber}]", fontHeader, XBrushes.DodgerBlue, new XRect(50, 120, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"NAME: {nameWithTitle}", fontText, XBrushes.Black, new XRect(50, 160, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"IDENTIFICATION NUMBER: {idNumber}", fontText, XBrushes.Black, new XRect(50, 200, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"ORIGIN: {originCityCountry}", fontText, XBrushes.Black, new XRect(50, 240, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"DESTINATION: {destinationCityCountry}", fontText, XBrushes.Black, new XRect(50, 280, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"DEPARTURE: {departure:dd MMM yyyy HH:mm}", fontText, XBrushes.Black, new XRect(50, 320, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"ARRIVAL: {arrival:dd MMM yyyy HH:mm}", fontText, XBrushes.Black, new XRect(50, 360, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"SEAT: {seat}", fontText, XBrushes.Black, new XRect(50, 400, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"BOARDING TIME: {boardingTime:HH:mm}", fontText, XBrushes.Black, new XRect(50, 440, page.Width.Point - 100, 40), XStringFormats.TopLeft);

            // Draw a line to separate
            gfx.DrawLine(XPens.Black, 50, 480, page.Width.Point - 50, 480);

            // Add some extra text
            gfx.DrawString("Enjoy your flight!", fontText, XBrushes.Black, new XRect(50, 500, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString("Aero Info System - AIS", fontText, XBrushes.DodgerBlue, new XRect(50, 540, page.Width.Point - 100, 40), XStringFormats.TopLeft);

            // Draw a blue border around the page
            var borderPen = new XPen(XColors.DodgerBlue, 7);
            gfx.DrawRectangle(borderPen, 25, 25, page.Width.Point - 50, page.Height.Point - 50); // Draw rectangle with margin

            // Load the QR code image from the MemoryStream
            qrCode.Position = 0;
            XImage qrCodeImage = XImage.FromStream(qrCode);
            gfx.DrawImage(qrCodeImage, page.Width.Point / 2 - 80, 590, 160, 160); // Example position and size

            // Save the document to a MemoryStream
            var memoryStream = new MemoryStream();
            document.Save(memoryStream, false);
            memoryStream.Position = 0;

            return memoryStream; // Return the PDF memory stream
        }

        /// <summary>
        /// Generate a PDF for the Ticket Invoice
        /// </summary>
        /// <param name="name">Name of User</param>
        /// <param name="flightNumber">Flight Number</param>
        /// <param name="idNumber">User Identification Number</param>
        /// <param name="price">Ticket Price</param>
        /// <returns>Invoice PDF MemoryStream</returns>
        public MemoryStream GenerateInvoicePdf(string name, string flightNumber, decimal price, bool userCancel, bool flightCancel)
        {
            // Create a new PDF document
            var document = new PdfDocument();

            // Add a title (Metadata)
            if (userCancel || flightCancel)
            {
                document.Info.Title = $"Ticket Cancel of Flight {flightNumber}";
            }
            else
            {
                document.Info.Title = $"Invoice for Ticket of Flight {flightNumber}";
            }

            // Create an empty page
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            // Load and draw the AIS logo
            string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "AIS-logo.png"); // To get the path of the AIS logo, relative to wwwroot
            XImage logo = XImage.FromFile(imagePath);
            gfx.DrawImage(logo, 50, 50, 153, 69);

            // Set font
            var fontHeader = new XFont("Verdana", 16, XFontStyleEx.Bold);
            var fontText = new XFont("Verdana", 12, XFontStyleEx.Regular);

            if (userCancel)
            {
                // Draw information text
                gfx.DrawString($"TICKET CANCELED FOR FLIGHT [{flightNumber}]", fontHeader, XBrushes.DodgerBlue, new XRect(50, 120, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }
            else if (flightCancel)
            {
                gfx.DrawString($"FLIGHT CANCELED: [{flightNumber}]", fontHeader, XBrushes.DodgerBlue, new XRect(50, 120, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }
            else
            {
                gfx.DrawString($"INVOICE: TICKET OF FLIGHT [{flightNumber}]", fontHeader, XBrushes.DodgerBlue, new XRect(50, 120, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }

            // Draw a line to separate
            gfx.DrawLine(XPens.Black, 50, 310, page.Width.Point - 50, 310);

            gfx.DrawString($"CLIENT: {name}", fontText, XBrushes.Black, new XRect(50, 170, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            gfx.DrawString($"PRICE: {price:C2}", fontText, XBrushes.Black, new XRect(50, 220, page.Width.Point - 100, 40), XStringFormats.TopLeft);

            if (userCancel || flightCancel)
            {
                gfx.DrawString($"DATE OF CANCEL: {DateTime.UtcNow}", fontText, XBrushes.Black, new XRect(50, 270, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }
            else
            {
                gfx.DrawString($"DATE OF PURCHASE: {DateTime.UtcNow}", fontText, XBrushes.Black, new XRect(50, 270, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }

            // Draw a line to separate
            gfx.DrawLine(XPens.Black, 50, 310, page.Width.Point - 50, 310);

            if (userCancel)
            {
                gfx.DrawString($"REFUNDED VALUE: {price / 4:C2}", fontText, XBrushes.Black, new XRect(50, 330, page.Width.Point - 100, 40), XStringFormats.TopLeft);

                // Add some extra text
                gfx.DrawString("We will miss you in this flight!", fontText, XBrushes.Black, new XRect(50, 360, page.Width.Point - 100, 40), XStringFormats.TopLeft);
                gfx.DrawString("Aero Info System - AIS", fontText, XBrushes.DodgerBlue, new XRect(50, 400, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }
            else if (flightCancel)
            {
                gfx.DrawString($"REFUNDED VALUE: {price:C2}", fontText, XBrushes.Black, new XRect(50, 330, page.Width.Point - 100, 40), XStringFormats.TopLeft);

                // Add some extra text
                gfx.DrawString("We are very sorry for the flight cancel!", fontText, XBrushes.Black, new XRect(50, 360, page.Width.Point - 100, 40), XStringFormats.TopLeft);
                gfx.DrawString("Aero Info System - AIS", fontText, XBrushes.DodgerBlue, new XRect(50, 400, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }
            else
            {
                gfx.DrawString("Thank you for your purchase!", fontText, XBrushes.Black, new XRect(50, 350, page.Width.Point - 100, 40), XStringFormats.TopLeft);
                gfx.DrawString("Aero Info System - AIS", fontText, XBrushes.DodgerBlue, new XRect(50, 390, page.Width.Point - 100, 40), XStringFormats.TopLeft);
            }

            // Draw a blue border around the page
            var borderPen = new XPen(XColors.DodgerBlue, 7);
            gfx.DrawRectangle(borderPen, 25, 25, page.Width.Point - 50, page.Height.Point - 50); // Draw rectangle with margin

            // Save the document to a MemoryStream
            var memoryStream = new MemoryStream();
            document.Save(memoryStream, false);
            memoryStream.Position = 0;

            return memoryStream; // Return the PDF memory stream
        }
    }
}
