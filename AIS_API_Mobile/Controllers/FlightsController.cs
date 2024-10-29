using AIS_API_Mobile.Data.Entities;
using AIS_API_Mobile.Data.Repositories;
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
    public class FlightsController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ITicketRecordRepository _ticketRecordRepository;
        private readonly IFlightRepository _flightRepository;

        public FlightsController(IUserHelper userHelper, ITicketRecordRepository ticketRecordRepository, IFlightRepository flightRepository)
        {
            _userHelper = userHelper;
            _ticketRecordRepository = ticketRecordRepository;
            _flightRepository = flightRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserFlightHistory()
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            List<TicketRecord> ticketRecords = await _ticketRecordRepository.GetAll().Where(t => t.UserId == user.Id && t.Departure < DateTime.UtcNow.AddHours(1)).ToListAsync();

            return Ok(ticketRecords);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserScheduledFlights()
        {
            // Get user email from claims passed through bearer
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Get the user
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            List<TicketRecord> scheduledFlights = await _ticketRecordRepository.GetAll().Where(t => t.UserId == user.Id && t.Departure >= DateTime.UtcNow.AddHours(1) && t.Canceled == false).ToListAsync();

            return Ok(scheduledFlights);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAvailableFlights()
        {
            List<FlightModel> availableFlights = new();
            List<Flight> flights = await _flightRepository.GetFlightsIncludeAsync();
            flights = flights.Where(f => f.AvailableSeats.Count > 0 && f.Departure >= DateTime.UtcNow.AddHours(1)).ToList();

            foreach (var flight in flights)
            {
                availableFlights.Add(new FlightModel
                {
                    Id = flight.Id,
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
                    FlightCapacity = flight.Aircraft.Capacity,
                    AvailableSeats = flight.AvailableSeats,
                });
            };

            return Ok(availableFlights);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetFlightById(int? id)
        {
            if (id == null)
            {
                return BadRequest("Invalid flight ID!");
            }

            var flight = await _flightRepository.GetFlightIncludeByIdAsync(id.Value);

            if (flight == null)
            {
                return NotFound("Flight not found!");
            }

            FlightModel flightModel = new FlightModel
            {
                Id = flight.Id,
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
                FlightCapacity = flight.Aircraft.Capacity,
                AvailableSeats = flight.AvailableSeats,
            };

            return Ok(flightModel);
        }
    }
}
