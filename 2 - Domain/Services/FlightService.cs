using Cocus.Domain.DTOs;
using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Services;
using Cocus.Domain.Interfaces.Repositories;

namespace Cocus.Domain.Services;

public class FlightService : IFlightService
{
	private readonly IFlightRepository _flightRepository;
	private readonly IAirportRepository _airportRepository;
	private readonly IAircraftRepository _aircraftRepository;

	public FlightService(
		IFlightRepository flightRepository,
		IAirportRepository airportRepository,
		IAircraftRepository aircraftRepository)
	{
		_flightRepository = flightRepository;
		_airportRepository = airportRepository;
		_aircraftRepository = aircraftRepository;
	}

	public async Task<FlightDto?> GetByIdAsync(int id)
	{
		var flight = await _flightRepository.GetByIdWithDetailsAsync(id);
		return flight == null ? null : await MapToDtoAsync(flight);
	}

	public async Task<IEnumerable<FlightDto>> GetAllAsync()
	{
		var flights = await _flightRepository.GetAllWithDetailsAsync();
		var dtos = new List<FlightDto>();

		foreach (var flight in flights)
		{
			dtos.Add(await MapToDtoAsync(flight));
		}

		return dtos;
	}

	public async Task<FlightDto?> GetByFlightNumberAsync(string flightNumber)
	{
		var flight = await _flightRepository.GetByFlightNumberAsync(flightNumber);
		return flight == null ? null : await MapToDtoAsync(flight);
	}

	public async Task<FlightDto> CreateAsync(FlightDto flightDto)
	{
		// Get related entities
		var departureAirport = await _airportRepository.GetByIdAsync(flightDto.DepartureAirportId);
		var destinationAirport = await _airportRepository.GetByIdAsync(flightDto.DestinationAirportId);
		var aircraft = await _aircraftRepository.GetByIdAsync(flightDto.AircraftId);

		if (departureAirport == null || destinationAirport == null || aircraft == null)
			throw new InvalidOperationException("Invalid airport or aircraft selection");

		// Calculate flight metrics
		var distance = CalculateDistance(
			departureAirport.Latitude, departureAirport.Longitude,
			destinationAirport.Latitude, destinationAirport.Longitude);

		var fuelRequired = CalculateFuelRequired(
			distance,
			aircraft.FuelConsumptionPerKm,
			aircraft.TakeoffFuelEffort);

		var flightTime = CalculateFlightTime(distance, aircraft.CruiseSpeedKmh);

		var flight = new Flight
		{
			FlightNumber = flightDto.FlightNumber,
			DepartureAirportId = flightDto.DepartureAirportId,
			DestinationAirportId = flightDto.DestinationAirportId,
			AircraftId = flightDto.AircraftId,
			ScheduledDeparture = flightDto.ScheduledDeparture,
			ActualDeparture = flightDto.ActualDeparture,
			ActualArrival = flightDto.ActualArrival,
			DistanceKm = distance,
			FuelRequiredLiters = fuelRequired,
			EstimatedFlightTimeHours = flightTime,
			Status = flightDto.Status,
			CreatedAt = DateTime.UtcNow
		};

		var created = await _flightRepository.AddAsync(flight);
		return await MapToDtoAsync(created);
	}

	public async Task UpdateAsync(FlightDto flightDto)
	{
		var flight = await _flightRepository.GetByIdAsync(flightDto.Id);
		if (flight == null)
			throw new InvalidOperationException($"Flight with ID {flightDto.Id} not found");

		// Get related entities for recalculation
		var departureAirport = await _airportRepository.GetByIdAsync(flightDto.DepartureAirportId);
		var destinationAirport = await _airportRepository.GetByIdAsync(flightDto.DestinationAirportId);
		var aircraft = await _aircraftRepository.GetByIdAsync(flightDto.AircraftId);

		if (departureAirport == null || destinationAirport == null || aircraft == null)
			throw new InvalidOperationException("Invalid airport or aircraft selection");

		// Recalculate if airports or aircraft changed
		var distance = CalculateDistance(
			departureAirport.Latitude, departureAirport.Longitude,
			destinationAirport.Latitude, destinationAirport.Longitude);

		var fuelRequired = CalculateFuelRequired(
			distance,
			aircraft.FuelConsumptionPerKm,
			aircraft.TakeoffFuelEffort);

		var flightTime = CalculateFlightTime(distance, aircraft.CruiseSpeedKmh);

		flight.FlightNumber = flightDto.FlightNumber;
		flight.DepartureAirportId = flightDto.DepartureAirportId;
		flight.DestinationAirportId = flightDto.DestinationAirportId;
		flight.AircraftId = flightDto.AircraftId;
		flight.ScheduledDeparture = flightDto.ScheduledDeparture;
		flight.ActualDeparture = flightDto.ActualDeparture;
		flight.ActualArrival = flightDto.ActualArrival;
		flight.DistanceKm = distance;
		flight.FuelRequiredLiters = fuelRequired;
		flight.EstimatedFlightTimeHours = flightTime;
		flight.Status = flightDto.Status;
		flight.UpdatedAt = DateTime.UtcNow;

		await _flightRepository.UpdateAsync(flight);
	}

	public async Task DeleteAsync(int id)
	{
		await _flightRepository.DeleteAsync(id);
	}

	public async Task<bool> ExistsAsync(int id)
	{
		return await _flightRepository.ExistsAsync(id);
	}

	public async Task<FlightReportDto> GetFlightReportAsync()
	{
		var flights = await _flightRepository.GetAllWithDetailsAsync();
		var flightList = flights.ToList();

		var reportItems = flightList.Select(f => new FlightReportItemDto
		{
			FlightNumber = f.FlightNumber,
			DepartureAirportCode = f.DepartureAirport.Code,
			DepartureAirportName = f.DepartureAirport.Name,
			DestinationAirportCode = f.DestinationAirport.Code,
			DestinationAirportName = f.DestinationAirport.Name,
			AircraftModel = f.Aircraft.Model,
			AircraftRegistration = f.Aircraft.RegistrationNumber,
			ScheduledDeparture = f.ScheduledDeparture,
			DistanceKm = f.DistanceKm,
			EstimatedFlightTimeHours = f.EstimatedFlightTimeHours,
			FuelRequiredLiters = f.FuelRequiredLiters,
			Status = f.Status.ToString()
		}).ToList();

		return new FlightReportDto
		{
			Flights = reportItems,
			TotalFlights = flightList.Count,
			TotalDistanceKm = flightList.Sum(f => f.DistanceKm),
			TotalFuelLiters = flightList.Sum(f => f.FuelRequiredLiters),
			TotalFlightTimeHours = flightList.Sum(f => f.EstimatedFlightTimeHours),
			AverageDistanceKm = flightList.Any() ? flightList.Average(f => f.DistanceKm) : 0,
			AverageFuelLiters = flightList.Any() ? flightList.Average(f => f.FuelRequiredLiters) : 0
		};
	}

	public async Task<string?> ValidateFlightAsync(FlightDto flightDto)
	{
		if (flightDto.DepartureAirportId == flightDto.DestinationAirportId)
		{
			return "Destination airport must be different from departure airport.";
		}

		return null;
	}

	public async Task<FlightFormDataDto> GetFlightFormDataAsync()
	{
		var airports = await _airportRepository.GetAllAsync();
		var aircraft = await _aircraftRepository.GetAllAsync();

		return new FlightFormDataDto
		{
			DepartureAirports = airports.Select(a => new SelectListItemDto
			{
				Id = a.Id,
				Display = $"{a.Code} - {a.Name} ({a.City})"
			}).ToList(),
			DestinationAirports = airports.Select(a => new SelectListItemDto
			{
				Id = a.Id,
				Display = $"{a.Code} - {a.Name} ({a.City})"
			}).ToList(),
			Aircraft = aircraft.Select(a => new SelectListItemDto
			{
				Id = a.Id,
				Display = $"{a.Model} - {a.RegistrationNumber}"
			}).ToList()
		};
	}

	/// <summary>
	/// Calculate distance between two GPS coordinates using Haversine formula
	/// </summary>
	public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
	{
		const double R = 6371; // Earth's radius in kilometers

		var dLat = ToRadians(lat2 - lat1);
		var dLon = ToRadians(lon2 - lon1);

		var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
				Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

		var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

		return R * c;
	}

	/// <summary>
	/// Calculate fuel required for flight including takeoff effort
	/// </summary>
	public double CalculateFuelRequired(double distanceKm, double fuelConsumptionPerKm, double takeoffEffort)
	{
		return (distanceKm * fuelConsumptionPerKm) + takeoffEffort;
	}

	/// <summary>
	/// Calculate estimated flight time based on distance and cruise speed
	/// </summary>
	public double CalculateFlightTime(double distanceKm, double cruiseSpeed)
	{
		return distanceKm / cruiseSpeed;
	}

	private static double ToRadians(double degrees)
	{
		return degrees * Math.PI / 180;
	}

	private async Task<FlightDto> MapToDtoAsync(Flight flight)
	{
		var dto = new FlightDto
		{
			Id = flight.Id,
			FlightNumber = flight.FlightNumber,
			DepartureAirportId = flight.DepartureAirportId,
			DestinationAirportId = flight.DestinationAirportId,
			AircraftId = flight.AircraftId,
			ScheduledDeparture = flight.ScheduledDeparture,
			ActualDeparture = flight.ActualDeparture,
			ActualArrival = flight.ActualArrival,
			DistanceKm = flight.DistanceKm,
			EstimatedFlightTimeHours = flight.EstimatedFlightTimeHours,
			FuelRequiredLiters = flight.FuelRequiredLiters,
			Status = flight.Status,
			CreatedAt = flight.CreatedAt,
			UpdatedAt = flight.UpdatedAt
		};

		// Map related entities if loaded
		if (flight.DepartureAirport != null)
		{
			dto.DepartureAirport = MapAirportToDto(flight.DepartureAirport);
		}

		if (flight.DestinationAirport != null)
		{
			dto.DestinationAirport = MapAirportToDto(flight.DestinationAirport);
		}

		if (flight.Aircraft != null)
		{
			dto.Aircraft = MapAircraftToDto(flight.Aircraft);
		}

		return dto;
	}

	private static AirportDto MapAirportToDto(Airport airport)
	{
		return new AirportDto
		{
			Id = airport.Id,
			Code = airport.Code,
			Name = airport.Name,
			City = airport.City,
			Country = airport.Country,
			Latitude = airport.Latitude,
			Longitude = airport.Longitude
		};
	}

	private static AircraftDto MapAircraftToDto(Aircraft aircraft)
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
			CruiseSpeedKmh = aircraft.CruiseSpeedKmh
		};
	}
}
