namespace Cocus.Domain.Entities;

public class Airport : BaseEntity
{
	public string Code { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string Country { get; set; } = string.Empty;
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public ICollection<Flight> DepartureFlights { get; set; } = new List<Flight>();
	public ICollection<Flight> DestinationFlights { get; set; } = new List<Flight>();
}
