using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Repositories;

public class FlightRepository : Repository<Flight>, IFlightRepository
{
	public FlightRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<Flight?> GetByIdWithDetailsAsync(int id)
	{
		return await _dbSet
			.Include(f => f.DepartureAirport)
			.Include(f => f.DestinationAirport)
			.Include(f => f.Aircraft)
			.FirstOrDefaultAsync(f => f.Id == id);
	}

	public async Task<IEnumerable<Flight>> GetAllWithDetailsAsync()
	{
		return await _dbSet
			.Include(f => f.DepartureAirport)
			.Include(f => f.DestinationAirport)
			.Include(f => f.Aircraft)
			.OrderByDescending(f => f.ScheduledDeparture)
			.ToListAsync();
	}

	public async Task<IEnumerable<Flight>> GetFlightsByAirportAsync(int airportId)
	{
		return await _dbSet
			.Include(f => f.DepartureAirport)
			.Include(f => f.DestinationAirport)
			.Include(f => f.Aircraft)
			.Where(f => f.DepartureAirportId == airportId || f.DestinationAirportId == airportId)
			.ToListAsync();
	}

	public async Task<IEnumerable<Flight>> GetFlightsByAircraftAsync(int aircraftId)
	{
		return await _dbSet
			.Include(f => f.DepartureAirport)
			.Include(f => f.DestinationAirport)
			.Include(f => f.Aircraft)
			.Where(f => f.AircraftId == aircraftId)
			.ToListAsync();
	}

	public async Task<Flight?> GetByFlightNumberAsync(string flightNumber)
	{
		return await _dbSet
			.Include(f => f.DepartureAirport)
			.Include(f => f.DestinationAirport)
			.Include(f => f.Aircraft)
			.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber);
	}
}
