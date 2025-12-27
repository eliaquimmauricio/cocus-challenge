using Cocus.Domain.Entities;

namespace Cocus.Domain.Interfaces.Repositories;

public interface IFlightRepository : IRepository<Flight>
{
	Task<Flight?> GetByIdWithDetailsAsync(int id);
	Task<IEnumerable<Flight>> GetAllWithDetailsAsync();
	Task<IEnumerable<Flight>> GetFlightsByAirportAsync(int airportId);
	Task<IEnumerable<Flight>> GetFlightsByAircraftAsync(int aircraftId);
	Task<Flight?> GetByFlightNumberAsync(string flightNumber);
}
