using Cocus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cocus.Infra.Data.Mappings;

public class FlightMapping : IEntityTypeConfiguration<Flight>
{
	public void Configure(EntityTypeBuilder<Flight> builder)
	{
		builder.ToTable("Flights");

		builder.HasKey(f => f.Id);

		builder.Property(f => f.FlightNumber)
			.IsRequired()
			.HasMaxLength(20);

		builder.HasIndex(f => f.FlightNumber);

		builder.Property(f => f.ScheduledDeparture)
			.IsRequired();

		builder.Property(f => f.ActualDeparture)
			.IsRequired(false);

		builder.Property(f => f.ActualArrival)
			.IsRequired(false);

		builder.Property(f => f.DistanceKm)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(f => f.EstimatedFlightTimeHours)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(f => f.FuelRequiredLiters)
			.IsRequired()
			.HasPrecision(10, 2);

		builder.Property(f => f.Status)
			.IsRequired()
			.HasConversion<int>();

		builder.Property(f => f.CreatedAt)
			.IsRequired();

		builder.Property(f => f.UpdatedAt)
			.IsRequired(false);

		// Relationships are configured in Airport and Aircraft mappings
	}
}
