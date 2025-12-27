using System.ComponentModel.DataAnnotations;

namespace Cocus.Domain.DTOs;

public class AircraftDto
{
	public int Id { get; set; }

	[Required(ErrorMessage = "Aircraft model is required")]
	[StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
	public string Model { get; set; } = string.Empty;

	[Required(ErrorMessage = "Manufacturer is required")]
	[StringLength(100, ErrorMessage = "Manufacturer cannot exceed 100 characters")]
	public string Manufacturer { get; set; } = string.Empty;

	[Required(ErrorMessage = "Registration number is required")]
	[StringLength(20, ErrorMessage = "Registration number cannot exceed 20 characters")]
	public string RegistrationNumber { get; set; } = string.Empty;

	[Required(ErrorMessage = "Fuel consumption per km is required")]
	[Range(0.1, 1000, ErrorMessage = "Fuel consumption must be between 0.1 and 1000 liters/km")]
	public double FuelConsumptionPerKm { get; set; }

	[Required(ErrorMessage = "Takeoff fuel effort is required")]
	[Range(1, 100000, ErrorMessage = "Takeoff fuel effort must be between 1 and 100000 liters")]
	public double TakeoffFuelEffort { get; set; }

	[Required(ErrorMessage = "Maximum range is required")]
	[Range(10, 50000, ErrorMessage = "Maximum range must be between 10 and 50000 km")]
	public double MaxRangeKm { get; set; }

	[Required(ErrorMessage = "Cruise speed is required")]
	[Range(100, 3000, ErrorMessage = "Cruise speed must be between 100 and 3000 km/h")]
	public double CruiseSpeedKmh { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
