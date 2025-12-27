using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Repositories;

public class AircraftRepository : Repository<Aircraft>, IAircraftRepository
{
	public AircraftRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<Aircraft?> GetByRegistrationNumberAsync(string registrationNumber)
	{
		return await _dbSet.FirstOrDefaultAsync(a => a.RegistrationNumber == registrationNumber);
	}

	public async Task<IEnumerable<Aircraft>> GetAvailableAircraftAsync()
	{
		// In a real system, this would filter based on maintenance schedules, etc.
		// For now, return all aircraft
		return await _dbSet.ToListAsync();
	}
}
