using Cocus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Context;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<Product> Products { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Airport> Airports { get; set; }
	public DbSet<Aircraft> Aircraft { get; set; }
	public DbSet<Flight> Flights { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	}
}
