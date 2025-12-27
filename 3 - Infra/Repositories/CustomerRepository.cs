using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
	public CustomerRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<Customer?> GetByEmailAsync(string email)
	{
		return await _dbSet
			.FirstOrDefaultAsync(c => c.Email == email);
	}

	public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
	{
		return await _dbSet
			.Where(c => c.IsActive)
			.OrderBy(c => c.Name)
			.ToListAsync();
	}
}
