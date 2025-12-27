namespace Cocus.Domain.Entities;

public class Aircraft : BaseEntity
{
	public string Model { get; set; } = string.Empty;
	public string Manufacturer { get; set; } = string.Empty;
	public string RegistrationNumber { get; set; } = string.Empty;
	public double FuelConsumptionPerKm { get; set; }
	public double TakeoffFuelEffort { get; set; }
	public double MaxRangeKm { get; set; }
	public double CruiseSpeedKmh { get; set; }
	public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
