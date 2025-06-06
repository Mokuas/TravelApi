using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trip.Models;
using Trip.Models.DTOs;

namespace Trip.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripDbContext _context;

    public TripsController(TripDbContext context)
    {
        _context = context;
    }

    // GET /api/trips?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var totalTrips = await _context.Trip.CountAsync();
        var totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var trips = await _context.Trip
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.Client)
            .Include(t => t.CountryTrips).ThenInclude(ct => ct.Country)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new {
            pageNum = page,
            pageSize = pageSize,
            allPages = totalPages,
            trips = trips.Select(t => new {
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                Countries = t.CountryTrips.Select(ct => new { ct.Country.Name }),
                Clients = t.ClientTrips.Select(ct => new { ct.Client.FirstName, ct.Client.LastName })
            })
        };

        return Ok(result);
    }

    // DELETE /api/clients/{idClient}
    [HttpDelete("/api/clients/{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var client = await _context.Client
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            return NotFound($"Client with ID {idClient} not found.");

        if (client.ClientTrips.Any())
            return BadRequest("Cannot delete client assigned to at least one trip.");

        _context.Client.Remove(client);
        await _context.SaveChangesAsync();

        return Ok($"Client with ID {idClient} deleted successfully.");
    }

    // POST /api/trips/{idTrip}/clients
    [HttpPost("/api/trips/{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] ClientTripDto dto)
    {
        var trip = await _context.Trip.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        if (trip == null)
            return NotFound("Trip not found.");

        if (trip.DateFrom <= DateTime.Now)
            return BadRequest("Cannot register for a trip that has already occurred.");

        var existingClient = await _context.Client.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
        if (existingClient != null)
        {
            var alreadyRegistered = await _context.Client_Trip
                .AnyAsync(ct => ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip);

            if (alreadyRegistered)
                return BadRequest("Client is already registered for this trip.");

            return BadRequest("Client with given PESEL already exists.");
        }

        var newClient = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };

        _context.Client.Add(newClient);
        await _context.SaveChangesAsync();

        var clientTrip = new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        _context.Client_Trip.Add(clientTrip);
        await _context.SaveChangesAsync();

        return Ok("Client assigned to trip successfully.");
    }
}
