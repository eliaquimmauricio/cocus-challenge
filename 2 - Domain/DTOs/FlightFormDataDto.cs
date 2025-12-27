namespace Cocus.Domain.DTOs;

public class FlightFormDataDto
{
	public IEnumerable<SelectListItemDto> DepartureAirports { get; set; } = new List<SelectListItemDto>();
	public IEnumerable<SelectListItemDto> DestinationAirports { get; set; } = new List<SelectListItemDto>();
	public IEnumerable<SelectListItemDto> Aircraft { get; set; } = new List<SelectListItemDto>();
}
