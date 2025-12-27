using Cocus.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cocus.Domain.DTOs;

public class FlightDto
{
	public int Id { get; set; }

	[Required(ErrorMessage = "Flight number is required")]
	[StringLength(20, ErrorMessage = "Flight number cannot exceed 20 characters")]
	public string FlightNumber { get; set; } = string.Empty;

	[Required(ErrorMessage = "Departure airport is required")]
	public int DepartureAirportId { get; set; }

	[Required(ErrorMessage = "Destination airport is required")]
	public int DestinationAirportId { get; set; }

	[Required(ErrorMessage = "Aircraft is required")]
	public int AircraftId { get; set; }

	[Required(ErrorMessage = "Scheduled departure is required")]
	public DateTime ScheduledDeparture { get; set; }
	public DateTime? ActualDeparture { get; set; }
	public DateTime? ActualArrival { get; set; }
	public double DistanceKm { get; set; }
	public double EstimatedFlightTimeHours { get; set; }
	public double FuelRequiredLiters { get; set; }
	public FlightStatus Status { get; set; }
	public AirportDto? DepartureAirport { get; set; }
	public AirportDto? DestinationAirport { get; set; }
	public AircraftDto? Aircraft { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
