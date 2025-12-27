using System.ComponentModel.DataAnnotations;

namespace Cocus.Domain.DTOs;

public class AirportDto
{
	public int Id { get; set; }

	[Required(ErrorMessage = "Airport code is required")]
	[StringLength(10, ErrorMessage = "Airport code cannot exceed 10 characters")]
	public string Code { get; set; } = string.Empty;

	[Required(ErrorMessage = "Airport name is required")]
	[StringLength(200, ErrorMessage = "Airport name cannot exceed 200 characters")]
	public string Name { get; set; } = string.Empty;

	[Required(ErrorMessage = "City is required")]
	[StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
	public string City { get; set; } = string.Empty;

	[Required(ErrorMessage = "Country is required")]
	[StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
	public string Country { get; set; } = string.Empty;

	[Required(ErrorMessage = "Latitude is required")]
	[Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
	public double Latitude { get; set; }

	[Required(ErrorMessage = "Longitude is required")]
	[Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
	public double Longitude { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}
