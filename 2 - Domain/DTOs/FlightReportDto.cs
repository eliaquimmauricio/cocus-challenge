namespace Cocus.Domain.DTOs;

public class FlightReportDto
{
	public IEnumerable<FlightReportItemDto> Flights { get; set; } = new List<FlightReportItemDto>();

	public int TotalFlights { get; set; }
	public double TotalDistanceKm { get; set; }
	public double TotalFuelLiters { get; set; }
	public double TotalFlightTimeHours { get; set; }
	public double AverageDistanceKm { get; set; }
	public double AverageFuelLiters { get; set; }
}

public class FlightReportItemDto
{
	public string FlightNumber { get; set; } = string.Empty;
	public string DepartureAirportCode { get; set; } = string.Empty;
	public string DepartureAirportName { get; set; } = string.Empty;
	public string DestinationAirportCode { get; set; } = string.Empty;
	public string DestinationAirportName { get; set; } = string.Empty;
	public string AircraftModel { get; set; } = string.Empty;
	public string AircraftRegistration { get; set; } = string.Empty;
	public DateTime ScheduledDeparture { get; set; }
	public double DistanceKm { get; set; }
	public double EstimatedFlightTimeHours { get; set; }
	public double FuelRequiredLiters { get; set; }
	public string Status { get; set; } = string.Empty;
}
