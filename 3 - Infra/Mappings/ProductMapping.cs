using Cocus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cocus.Infra.Data.Mappings;

public class ProductMapping : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("Products");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(p => p.Description)
			.HasMaxLength(1000);

		builder.Property(p => p.Price)
			.IsRequired()
			.HasPrecision(18, 2);

		builder.Property(p => p.Stock)
			.IsRequired();

		builder.Property(p => p.IsActive)
			.IsRequired();

		builder.Property(p => p.CreatedAt)
			.IsRequired();

		builder.Property(p => p.UpdatedAt);
	}
}
