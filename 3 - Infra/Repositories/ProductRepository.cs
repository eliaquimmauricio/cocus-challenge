using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
	public ProductRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<IEnumerable<Product>> GetActiveProductsAsync()
	{
		return await _dbSet
			.Where(p => p.IsActive)
			.OrderBy(p => p.Name)
			.ToListAsync();
	}

	public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
	{
		return await _dbSet
			.Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive)
			.OrderBy(p => p.Price)
			.ToListAsync();
	}
}
