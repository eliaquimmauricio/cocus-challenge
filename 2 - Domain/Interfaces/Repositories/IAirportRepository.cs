using Cocus.Domain.Entities;

namespace Cocus.Domain.Interfaces.Repositories;

public interface IAirportRepository : IRepository<Airport>
{
	Task<Airport?> GetByCodeAsync(string code);
	Task<IEnumerable<Airport>> SearchByNameOrCityAsync(string searchTerm);
}
