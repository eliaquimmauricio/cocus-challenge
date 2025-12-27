using Cocus.Domain.Entities;

namespace Cocus.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
	Task<IEnumerable<Product>> GetActiveProductsAsync();
	Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}
