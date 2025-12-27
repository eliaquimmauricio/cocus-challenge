using Cocus.Domain.DTOs;

namespace Cocus.Domain.Interfaces.Services;

public interface IAircraftService
{
	Task<AircraftDto?> GetByIdAsync(int id);
	Task<IEnumerable<AircraftDto>> GetAllAsync();
	Task<AircraftDto?> GetByRegistrationNumberAsync(string registrationNumber);
	Task<AircraftDto> CreateAsync(AircraftDto aircraftDto);
	Task UpdateAsync(AircraftDto aircraftDto);
	Task DeleteAsync(int id);
	Task<bool> ExistsAsync(int id);
	Task<IEnumerable<AircraftDto>> GetAvailableAircraftAsync();
	Task<string?> ValidateAircraftAsync(AircraftDto aircraftDto);
}
