using Cocus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cocus.Infra.Data.Mappings;

public class CustomerMapping : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.ToTable("Customers");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(c => c.Email)
			.IsRequired()
			.HasMaxLength(100);

		builder.HasIndex(c => c.Email)
			.IsUnique();

		builder.Property(c => c.Phone)
			.HasMaxLength(20);

		builder.Property(c => c.Address)
			.HasMaxLength(500);

		builder.Property(c => c.IsActive)
			.IsRequired();

		builder.Property(c => c.CreatedAt)
			.IsRequired();

		builder.Property(c => c.UpdatedAt);
	}
}
