using Cocus.Domain.Entities;

namespace Cocus.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
	Task<Customer?> GetByEmailAsync(string email);
	Task<IEnumerable<Customer>> GetActiveCustomersAsync();
}
