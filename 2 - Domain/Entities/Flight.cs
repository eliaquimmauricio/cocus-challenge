namespace Cocus.Domain.Entities;

public class Flight : BaseEntity
{
	public string FlightNumber { get; set; } = string.Empty;
	public int DepartureAirportId { get; set; }
	public int DestinationAirportId { get; set; }
	public int AircraftId { get; set; }
	public DateTime ScheduledDeparture { get; set; }
	public DateTime? ActualDeparture { get; set; }
	public DateTime? ActualArrival { get; set; }
	public double DistanceKm { get; set; }
	public double EstimatedFlightTimeHours { get; set; }
	public double FuelRequiredLiters { get; set; }
	public FlightStatus Status { get; set; }
	public Airport DepartureAirport { get; set; } = null!;
	public Airport DestinationAirport { get; set; } = null!;
	public Aircraft Aircraft { get; set; } = null!;
}

public enum FlightStatus
{
	Scheduled = 0,
	Boarding = 1,
	Departed = 2,
	InFlight = 3,
	Landed = 4,
	Cancelled = 5
}
