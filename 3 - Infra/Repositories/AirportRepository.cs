using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Repositories;

public class AirportRepository : Repository<Airport>, IAirportRepository
{
	public AirportRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<Airport?> GetByCodeAsync(string code)
	{
		return await _dbSet.FirstOrDefaultAsync(a => a.Code == code);
	}

	public async Task<IEnumerable<Airport>> SearchByNameOrCityAsync(string searchTerm)
	{
		var lowerSearch = searchTerm.ToLower();
		return await _dbSet
			.Where(a => a.Name.ToLower().Contains(lowerSearch) ||
					   a.City.ToLower().Contains(lowerSearch) ||
					   a.Code.ToLower().Contains(lowerSearch))
			.ToListAsync();
	}
}
