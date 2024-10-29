using AIS_API_Mobile.Data.Entities;
using AIS_API_Mobile.Data.Repositories;
using AIS_API_Mobile.HelperClasses;
using AIS_API_Mobile.Helpers;
using AIS_API_Mobile.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AIS_API_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : Controller
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IUserHelper _userHelper;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketRecordRepository _ticketRecordRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IPdfGenerator _pdfGenerator;
        private readonly IQrCodeGenerator _qrCodeGenerator;

        public TicketsController(IFlightRepository flightRepository, IUserHelper userHelper, ITicketRepository ticketRepository, ITicketRecordRepository ticketRecordRepository, IMailHelper mailHelper, IPdfGenerator pdfGenerator, IQrCodeGenerator qrCodeGenerator)
        {
            _flightRepository = flightRepository;
            _userHelper = userHelper;
            _ticketRepository = ticketRepository;
            _ticketRecordRepository = ticketRecordRepository;
            _mailHelper = mailHelper;
            _pdfGenerator = pdfGenerator;
            _qrCodeGenerator = qrCodeGenerator;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]")]
        public async Task<IActionResult> PurchaseTicket([FromBody] PurchaseTicketModel model)
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            Flight flight = await _flightRepository.GetFlightIncludeByIdTrackAsync(model.FlightId);

            if (flight == null)
            {
                return NotFound("Flight not found!");
            }

            if (flight.AvailableSeats.Count < 1)
            {
                return NotFound("Flight has no available seats!");
            }

            foreach (Ticket checkTicket in flight.TicketList)
            {
                if (checkTicket.IdNumber == model.IdNumber)
                {
                    return BadRequest("There is already a ticket with this ID in this Flight!");
                }
            }

            var ticketPrice = TicketPriceGenerator.GetTicketPrice((flight.Arrival - flight.Departure), flight.AvailableSeats.Count, flight.Aircraft.Seats.Count);

            Ticket ticket = new()
            {
                User = user,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Flight = flight,
                Seat = model.Seat,
                Title = model.Title,
                FullName = model.FullName,
                IdNumber = model.IdNumber,
                Price = ticketPrice,
            };

            try
            {
                // Last check to see if seat is still available
                if (flight.AvailableSeats.Contains(ticket.Seat))
                {
                    // Create the ticket in the database
                    await _ticketRepository.CreateAsync(ticket);

                    // Update the flights internal ticketlist
                    flight.UpdateTicketList(ticket, false);

                    // Update the flight in the database
                    await _flightRepository.UpdateAsync(flight);

                    TicketRecord record = new TicketRecord
                    {
                        Id = ticket.Id,
                        UserId = user.Id,
                        Seat = ticket.Seat,
                        TicketPrice = ticket.Price,
                        FlightNumber = flight.FlightNumber,
                        OriginCity = flight.Origin.City,
                        OriginCountry = flight.Origin.Country,
                        OriginFlagImageUrl = flight.Origin.ImageUrl,
                        DestinationCity = flight.Destination.City,
                        DestinationCountry = flight.Destination.Country,
                        DestinationFlagImageUrl = flight.Destination.ImageUrl,
                        Departure = flight.Departure,
                        Arrival = flight.Arrival,
                        Canceled = false,
                        HolderIdNumber = model.IdNumber,
                    };

                    await _ticketRecordRepository.CreateAsync(record);

                    // Send ticket invoice to the email of user that bought the ticket
                    string emailBodyInvoice = _mailHelper.GetHtmlTemplateInvoice($"{user.FirstName} {user.LastName}", flight.FlightNumber, ticketPrice);
                    MemoryStream pdfInvoice = _pdfGenerator.GenerateInvoicePdf($"{user.FirstName} {user.LastName}", flight.FlightNumber, ticketPrice, false, false);
                    Response responseInvoice = await _mailHelper.SendEmailAsync(user.Email, $"Invoice Ticket ID-{ticket.Id}", emailBodyInvoice, pdfInvoice, $"ticket_invoice_flight_{flight.FlightNumber}_{user.FirstName}_{user.LastName}.pdf", null);

                    if (!responseInvoice.IsSuccess)
                    {
                        return StatusCode(500, "Invoice mailing error!");

                    }

                    // Send the ticket itself to the email of the ticket holder that was inserted when buying the ticket
                    MemoryStream qrCode = _qrCodeGenerator.GenerateQrCode($"VALID TICKET: Flight {flight.FlightNumber} - Passenger {model.Title} {model.FullName} - Identification Number: {model.IdNumber}");
                    string emailBodyTicket = _mailHelper.GetHtmlTemplateTicket("Ticket", $"{model.Title} {model.FullName}", model.IdNumber, flight.FlightNumber, $"{flight.Origin.City}, {flight.Origin.Country}", $"{flight.Destination.City}, {flight.Destination.Country}", model.Seat, flight.Departure, flight.Arrival, false);
                    MemoryStream pdfTicket = _pdfGenerator.GenerateTicketPdf($"{model.Title} {model.FullName}", model.IdNumber, flight.FlightNumber, $"{flight.Origin.City}, {flight.Origin.Country}", $"{flight.Destination.City}, {flight.Destination.Country}", model.Seat, flight.Departure, flight.Arrival, qrCode);
                    Response responseTicket = await _mailHelper.SendEmailAsync(model.Email, $"Ticket ID-{ticket.Id}", emailBodyTicket, pdfTicket, $"ticket_flight_{flight.FlightNumber}_{model.IdNumber}.pdf", qrCode);

                    if (!responseTicket.IsSuccess)
                    {
                        return StatusCode(500, "Ticket mailing error!");
                    }

                    return Ok("Ticket successfully purchased!");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, ex.Message);
            }

            return StatusCode(500, "An unexpected error ocurred while purchasing the ticket!");
        }
    }
}
