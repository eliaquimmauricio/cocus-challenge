using Cocus.Domain.DTOs;
using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Services;
using Cocus.Domain.Interfaces.Repositories;

namespace Cocus.Domain.Services;

public class AirportService : IAirportService
{
	private readonly IAirportRepository _airportRepository;

	public AirportService(IAirportRepository airportRepository)
	{
		_airportRepository = airportRepository;
	}

	public async Task<AirportDto?> GetByIdAsync(int id)
	{
		var airport = await _airportRepository.GetByIdAsync(id);
		return airport == null ? null : MapToDto(airport);
	}

	public async Task<IEnumerable<AirportDto>> GetAllAsync()
	{
		var airports = await _airportRepository.GetAllAsync();
		return airports.Select(MapToDto);
	}

	public async Task<AirportDto?> GetByCodeAsync(string code)
	{
		var airport = await _airportRepository.GetByCodeAsync(code);
		return airport == null ? null : MapToDto(airport);
	}

	public async Task<AirportDto> CreateAsync(AirportDto airportDto)
	{
		var airport = MapToEntity(airportDto);
		airport.CreatedAt = DateTime.UtcNow;

		var created = await _airportRepository.AddAsync(airport);
		return MapToDto(created);
	}

	public async Task UpdateAsync(AirportDto airportDto)
	{
		var airport = await _airportRepository.GetByIdAsync(airportDto.Id);
		if (airport == null)
			throw new InvalidOperationException($"Airport with ID {airportDto.Id} not found");

		airport.Code = airportDto.Code;
		airport.Name = airportDto.Name;
		airport.City = airportDto.City;
		airport.Country = airportDto.Country;
		airport.Latitude = airportDto.Latitude;
		airport.Longitude = airportDto.Longitude;
		airport.UpdatedAt = DateTime.UtcNow;

		await _airportRepository.UpdateAsync(airport);
	}

	public async Task DeleteAsync(int id)
	{
		await _airportRepository.DeleteAsync(id);
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await _airportRepository.ExistsAsync(id);
	}

	public async Task<IEnumerable<AirportDto>> SearchAsync(string searchTerm)
	{
		var airports = await _airportRepository.SearchByNameOrCityAsync(searchTerm);
		return airports.Select(MapToDto);
	}

	public async Task<string?> ValidateAirportAsync(AirportDto airportDto)
	{
		// Check for duplicate airport code
		var existing = await _airportRepository.GetByCodeAsync(airportDto.Code);
		if (existing != null && existing.Id != airportDto.Id)
		{
			return $"An airport with code '{airportDto.Code}' already exists.";
		}

		return null;
	}

	private static AirportDto MapToDto(Airport airport)
	{
		return new AirportDto
		{
			Id = airport.Id,
			Code = airport.Code,
			Name = airport.Name,
			City = airport.City,
			Country = airport.Country,
			Latitude = airport.Latitude,
			Longitude = airport.Longitude,
			CreatedAt = airport.CreatedAt,
			UpdatedAt = airport.UpdatedAt
		};
	}

	private static Airport MapToEntity(AirportDto dto)
	{
		return new Airport
		{
			Id = dto.Id,
			Code = dto.Code,
			Name = dto.Name,
			City = dto.City,
			Country = dto.Country,
			Latitude = dto.Latitude,
			Longitude = dto.Longitude
		};
	}
}
