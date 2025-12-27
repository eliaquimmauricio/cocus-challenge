using Cocus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cocus.Infra.Data.Mappings;

public class AirportMapping : IEntityTypeConfiguration<Airport>
{
	public void Configure(EntityTypeBuilder<Airport> builder)
	{
		builder.ToTable("Airports");

		builder.HasKey(a => a.Id);

		builder.Property(a => a.Code)
			.IsRequired()
			.HasMaxLength(10);

		builder.HasIndex(a => a.Code)
			.IsUnique();

		builder.Property(a => a.Name)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(a => a.City)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(a => a.Country)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(a => a.Latitude)
			.IsRequired()
			.HasPrecision(10, 7);

		builder.Property(a => a.Longitude)
			.IsRequired()
			.HasPrecision(10, 7);

		builder.Property(a => a.CreatedAt)
			.IsRequired();

		builder.Property(a => a.UpdatedAt)
			.IsRequired(false);

		// Relationships
		builder.HasMany(a => a.DepartureFlights)
			.WithOne(f => f.DepartureAirport)
			.HasForeignKey(f => f.DepartureAirportId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasMany(a => a.DestinationFlights)
			.WithOne(f => f.DestinationAirport)
			.HasForeignKey(f => f.DestinationAirportId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
