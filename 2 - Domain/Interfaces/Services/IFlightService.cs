using Cocus.Domain.DTOs;

namespace Cocus.Domain.Interfaces.Services;

public interface IFlightService
{
	Task<FlightDto?> GetByIdAsync(int id);
	Task<IEnumerable<FlightDto>> GetAllAsync();
	Task<FlightDto?> GetByFlightNumberAsync(string flightNumber);
	Task<FlightDto> CreateAsync(FlightDto flightDto);
	Task UpdateAsync(FlightDto flightDto);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
	Task<FlightReportDto> GetFlightReportAsync();
	double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
	double CalculateFuelRequired(double distanceKm, double fuelConsumptionPerKm, double takeoffEffort);
	double CalculateFlightTime(double distanceKm, double cruiseSpeed);
	Task<string?> ValidateFlightAsync(FlightDto flightDto);
	Task<FlightFormDataDto> GetFlightFormDataAsync();
}
