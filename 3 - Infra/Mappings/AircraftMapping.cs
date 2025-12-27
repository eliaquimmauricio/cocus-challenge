using Cocus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cocus.Infra.Data.Mappings;

public class AircraftMapping : IEntityTypeConfiguration<Aircraft>
{
	public void Configure(EntityTypeBuilder<Aircraft> builder)
	{
		builder.ToTable("Aircraft");

		builder.HasKey(a => a.Id);

		builder.Property(a => a.Model)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(a => a.Manufacturer)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(a => a.RegistrationNumber)
			.IsRequired()
			.HasMaxLength(20);

		builder.HasIndex(a => a.RegistrationNumber)
			.IsUnique();

		builder.Property(a => a.FuelConsumptionPerKm)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(a => a.TakeoffFuelEffort)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(a => a.MaxRangeKm)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(a => a.CruiseSpeedKmh)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(a => a.CreatedAt)
			.IsRequired();

		builder.Property(a => a.UpdatedAt)
			.IsRequired(false);

		// Relationships
		builder.HasMany(a => a.Flights)
			.WithOne(f => f.Aircraft)
			.HasForeignKey(f => f.AircraftId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
