using Cocus.Domain.Entities;

namespace Cocus.Domain.Interfaces.Repositories;

public interface IAircraftRepository : IRepository<Aircraft>
{
	Task<Aircraft?> GetByRegistrationNumberAsync(string registrationNumber);
	Task<IEnumerable<Aircraft>> GetAvailableAircraftAsync();
}
