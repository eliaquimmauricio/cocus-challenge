using Cocus.Domain.DTOs;

namespace Cocus.Domain.Interfaces.Services;

public interface IAirportService
{
	Task<AirportDto?> GetByIdAsync(int id);
	Task<IEnumerable<AirportDto>> GetAllAsync();
	Task<AirportDto?> GetByCodeAsync(string code);
	Task<AirportDto> CreateAsync(AirportDto airportDto);
	Task UpdateAsync(AirportDto airportDto);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
	Task<IEnumerable<AirportDto>> SearchAsync(string searchTerm);
	Task<string?> ValidateAirportAsync(AirportDto airportDto);
}
