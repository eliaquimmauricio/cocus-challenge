using Cocus.Domain.DTOs;
using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Services;
using Cocus.Domain.Interfaces.Repositories;

namespace Cocus.Domain.Services;

public class AircraftService : IAircraftService
{
	private readonly IAircraftRepository _aircraftRepository;

	public AircraftService(IAircraftRepository aircraftRepository)
	{
		_aircraftRepository = aircraftRepository;
	}

	public async Task<AircraftDto?> GetByIdAsync(int id)
	{
		var aircraft = await _aircraftRepository.GetByIdAsync(id);
		return aircraft == null ? null : MapToDto(aircraft);
	}

	public async Task<IEnumerable<AircraftDto>> GetAllAsync()
	{
		var aircraft = await _aircraftRepository.GetAllAsync();
		return aircraft.Select(MapToDto);
	}

	public async Task<AircraftDto?> GetByRegistrationNumberAsync(string registrationNumber)
	{
		var aircraft = await _aircraftRepository.GetByRegistrationNumberAsync(registrationNumber);
		return aircraft == null ? null : MapToDto(aircraft);
	}

	public async Task<AircraftDto> CreateAsync(AircraftDto aircraftDto)
	{
		var aircraft = MapToEntity(aircraftDto);
		aircraft.CreatedAt = DateTime.UtcNow;

		var created = await _aircraftRepository.AddAsync(aircraft);
		return MapToDto(created);
	}

	public async Task UpdateAsync(AircraftDto aircraftDto)
	{
		var aircraft = await _aircraftRepository.GetByIdAsync(aircraftDto.Id);
		if (aircraft == null)
			throw new InvalidOperationException($"Aircraft with ID {aircraftDto.Id} not found");

		aircraft.Model = aircraftDto.Model;
		aircraft.Manufacturer = aircraftDto.Manufacturer;
		aircraft.RegistrationNumber = aircraftDto.RegistrationNumber;
		aircraft.FuelConsumptionPerKm = aircraftDto.FuelConsumptionPerKm;
		aircraft.TakeoffFuelEffort = aircraftDto.TakeoffFuelEffort;
		aircraft.MaxRangeKm = aircraftDto.MaxRangeKm;
		aircraft.CruiseSpeedKmh = aircraftDto.CruiseSpeedKmh;
		aircraft.UpdatedAt = DateTime.UtcNow;

		await _aircraftRepository.UpdateAsync(aircraft);
	}

	public async Task DeleteAsync(int id)
	{
		await _aircraftRepository.DeleteAsync(id);
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await _aircraftRepository.ExistsAsync(id);
	}

	public async Task<IEnumerable<AircraftDto>> GetAvailableAircraftAsync()
	{
		var aircraft = await _aircraftRepository.GetAvailableAircraftAsync();
		return aircraft.Select(MapToDto);
	}

	public async Task<string?> ValidateAircraftAsync(AircraftDto aircraftDto)
	{
		var existing = await _aircraftRepository.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber);

		if (existing != null && existing.Id != aircraftDto.Id)
		{
			return $"An aircraft with registration number '{aircraftDto.RegistrationNumber}' already exists.";
		}

		if (aircraftDto.FuelConsumptionPerKm <= 0)
		{
			return "Fuel consumption per km must be greater than zero.";
		}

		if (aircraftDto.MaxRangeKm <= 0)
		{
			return "Max range must be greater than zero.";
		}

		if (aircraftDto.CruiseSpeedKmh <= 0)
		{
			return "Cruise speed must be greater than zero.";
		}

		return null;
	}

	private static AircraftDto MapToDto(Aircraft aircraft)
	{
		return new AircraftDto
		{
			Id = aircraft.Id,
			Model = aircraft.Model,
			Manufacturer = aircraft.Manufacturer,
			RegistrationNumber = aircraft.RegistrationNumber,
			FuelConsumptionPerKm = aircraft.FuelConsumptionPerKm,
			TakeoffFuelEffort = aircraft.TakeoffFuelEffort,
			MaxRangeKm = aircraft.MaxRangeKm,
			CruiseSpeedKmh = aircraft.CruiseSpeedKmh,
			CreatedAt = aircraft.CreatedAt,
			UpdatedAt = aircraft.UpdatedAt
		};
	}

	private static Aircraft MapToEntity(AircraftDto dto)
	{
		return new Aircraft
		{
			Id = dto.Id,
			Model = dto.Model,
			Manufacturer = dto.Manufacturer,
			RegistrationNumber = dto.RegistrationNumber,
			FuelConsumptionPerKm = dto.FuelConsumptionPerKm,
			TakeoffFuelEffort = dto.TakeoffFuelEffort,
			MaxRangeKm = dto.MaxRangeKm,
			CruiseSpeedKmh = dto.CruiseSpeedKmh
		};
	}
}
