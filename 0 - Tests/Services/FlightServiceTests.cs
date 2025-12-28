using Bogus;
using Cocus.Domain.DTOs;
using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Domain.Services;
using Moq;

namespace Cocus.Tests.Services;

public class FlightServiceTests
{
	private readonly Mock<IFlightRepository> _flightRepositoryMock;
	private readonly Mock<IAirportRepository> _airportRepositoryMock;
	private readonly Mock<IAircraftRepository> _aircraftRepositoryMock;
	private readonly FlightService _flightService;
	private readonly Faker _faker;

	public FlightServiceTests()
	{
		_flightRepositoryMock = new Mock<IFlightRepository>();
		_airportRepositoryMock = new Mock<IAirportRepository>();
		_aircraftRepositoryMock = new Mock<IAircraftRepository>();
		_flightService = new FlightService(
			_flightRepositoryMock.Object,
			_airportRepositoryMock.Object,
			_aircraftRepositoryMock.Object);
		_faker = new Faker();
	}

	#region GetByIdAsync Tests

	[Fact]
	public async Task GetByIdAsync_WhenFlightExists_ReturnsFlightDto()
	{
		// Arrange
		var flight = GenerateFlight();
		_flightRepositoryMock
			.Setup(x => x.GetByIdWithDetailsAsync(flight.Id))
			.ReturnsAsync(flight);

		// Act
		var result = await _flightService.GetByIdAsync(flight.Id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(flight.Id, result.Id);
		Assert.Equal(flight.FlightNumber, result.FlightNumber);
		Assert.Equal(flight.DepartureAirportId, result.DepartureAirportId);
		Assert.Equal(flight.DestinationAirportId, result.DestinationAirportId);
		Assert.Equal(flight.AircraftId, result.AircraftId);
		_flightRepositoryMock.Verify(x => x.GetByIdWithDetailsAsync(flight.Id), Times.Once);
	}

	[Fact]
	public async Task GetByIdAsync_WhenFlightDoesNotExist_ReturnsNull()
	{
		// Arrange
		var flightId = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.GetByIdWithDetailsAsync(flightId))
			.ReturnsAsync((Flight?)null);

		// Act
		var result = await _flightService.GetByIdAsync(flightId);

		// Assert
		Assert.Null(result);
		_flightRepositoryMock.Verify(x => x.GetByIdWithDetailsAsync(flightId), Times.Once);
	}

	#endregion

	#region GetAllAsync Tests

	[Fact]
	public async Task GetAllAsync_WhenFlightsExist_ReturnsAllFlightDtos()
	{
		// Arrange
		var flights = GenerateFlights(5);
		_flightRepositoryMock
			.Setup(x => x.GetAllWithDetailsAsync())
			.ReturnsAsync(flights);

		// Act
		var result = await _flightService.GetAllAsync();

		// Assert
		var resultList = result.ToList();
		Assert.NotNull(result);
		Assert.Equal(flights.Count, resultList.Count);
		Assert.All(resultList, dto => Assert.NotNull(dto.FlightNumber));
		_flightRepositoryMock.Verify(x => x.GetAllWithDetailsAsync(), Times.Once);
	}

	[Fact]
	public async Task GetAllAsync_WhenNoFlightsExist_ReturnsEmptyCollection()
	{
		// Arrange
		_flightRepositoryMock
			.Setup(x => x.GetAllWithDetailsAsync())
			.ReturnsAsync(new List<Flight>());

		// Act
		var result = await _flightService.GetAllAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
		_flightRepositoryMock.Verify(x => x.GetAllWithDetailsAsync(), Times.Once);
	}

	#endregion

	#region GetByFlightNumberAsync Tests

	[Fact]
	public async Task GetByFlightNumberAsync_WhenFlightExists_ReturnsFlightDto()
	{
		// Arrange
		var flight = GenerateFlight();
		_flightRepositoryMock
			.Setup(x => x.GetByFlightNumberAsync(flight.FlightNumber))
			.ReturnsAsync(flight);

		// Act
		var result = await _flightService.GetByFlightNumberAsync(flight.FlightNumber);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(flight.FlightNumber, result.FlightNumber);
		_flightRepositoryMock.Verify(x => x.GetByFlightNumberAsync(flight.FlightNumber), Times.Once);
	}

	[Fact]
	public async Task GetByFlightNumberAsync_WhenFlightDoesNotExist_ReturnsNull()
	{
		// Arrange
		var flightNumber = _faker.Random.String2(6);
		_flightRepositoryMock
			.Setup(x => x.GetByFlightNumberAsync(flightNumber))
			.ReturnsAsync((Flight?)null);

		// Act
		var result = await _flightService.GetByFlightNumberAsync(flightNumber);

		// Assert
		Assert.Null(result);
		_flightRepositoryMock.Verify(x => x.GetByFlightNumberAsync(flightNumber), Times.Once);
	}

	#endregion

	#region CreateAsync Tests

	[Fact]
	public async Task CreateAsync_WithValidData_CreatesFlightWithCalculatedMetrics()
	{
		// Arrange
		var departureAirport = GenerateAirport();
		var destinationAirport = GenerateAirport();
		var aircraft = GenerateAircraft();
		var flightDto = GenerateFlightDto();
		flightDto.DepartureAirportId = departureAirport.Id;
		flightDto.DestinationAirportId = destinationAirport.Id;
		flightDto.AircraftId = aircraft.Id;

		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(departureAirport.Id))
			.ReturnsAsync(departureAirport);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(destinationAirport.Id))
			.ReturnsAsync(destinationAirport);
		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(aircraft.Id))
			.ReturnsAsync(aircraft);

		var createdFlight = new Flight
		{
			Id = _faker.Random.Int(1, 1000),
			FlightNumber = flightDto.FlightNumber,
			DepartureAirportId = flightDto.DepartureAirportId,
			DestinationAirportId = flightDto.DestinationAirportId,
			AircraftId = flightDto.AircraftId,
			ScheduledDeparture = flightDto.ScheduledDeparture,
			DepartureAirport = departureAirport,
			DestinationAirport = destinationAirport,
			Aircraft = aircraft
		};

		_flightRepositoryMock
			.Setup(x => x.AddAsync(It.IsAny<Flight>()))
			.ReturnsAsync(createdFlight);

		// Act
		var result = await _flightService.CreateAsync(flightDto);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(flightDto.FlightNumber, result.FlightNumber);
		Assert.Equal(flightDto.DepartureAirportId, result.DepartureAirportId);
		Assert.Equal(flightDto.DestinationAirportId, result.DestinationAirportId);
		Assert.Equal(flightDto.AircraftId, result.AircraftId);
		_flightRepositoryMock.Verify(x => x.AddAsync(It.Is<Flight>(f =>
			f.FlightNumber == flightDto.FlightNumber &&
			f.DistanceKm > 0 &&
			f.FuelRequiredLiters > 0 &&
			f.EstimatedFlightTimeHours > 0
		)), Times.Once);
	}

	[Fact]
	public async Task CreateAsync_WithInvalidDepartureAirport_ThrowsInvalidOperationException()
	{
		// Arrange
		var flightDto = GenerateFlightDto();
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(flightDto.DepartureAirportId))
			.ReturnsAsync((Airport?)null);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_flightService.CreateAsync(flightDto));
	}

	[Fact]
	public async Task CreateAsync_WithInvalidDestinationAirport_ThrowsInvalidOperationException()
	{
		// Arrange
		var departureAirport = GenerateAirport();
		var flightDto = GenerateFlightDto();
		flightDto.DepartureAirportId = departureAirport.Id;

		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(departureAirport.Id))
			.ReturnsAsync(departureAirport);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(flightDto.DestinationAirportId))
			.ReturnsAsync((Airport?)null);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_flightService.CreateAsync(flightDto));
	}

	[Fact]
	public async Task CreateAsync_WithInvalidAircraft_ThrowsInvalidOperationException()
	{
		// Arrange
		var departureAirport = GenerateAirport();
		var destinationAirport = GenerateAirport();
		var flightDto = GenerateFlightDto();
		flightDto.DepartureAirportId = departureAirport.Id;
		flightDto.DestinationAirportId = destinationAirport.Id;

		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(departureAirport.Id))
			.ReturnsAsync(departureAirport);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(destinationAirport.Id))
			.ReturnsAsync(destinationAirport);
		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(flightDto.AircraftId))
			.ReturnsAsync((Aircraft?)null);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_flightService.CreateAsync(flightDto));
	}

	#endregion

	#region UpdateAsync Tests

	[Fact]
	public async Task UpdateAsync_WithValidData_UpdatesFlightWithRecalculatedMetrics()
	{
		// Arrange
		var existingFlight = GenerateFlight();
		var departureAirport = GenerateAirport();
		var destinationAirport = GenerateAirport();
		var aircraft = GenerateAircraft();
		var flightDto = GenerateFlightDto();
		flightDto.Id = existingFlight.Id;
		flightDto.DepartureAirportId = departureAirport.Id;
		flightDto.DestinationAirportId = destinationAirport.Id;
		flightDto.AircraftId = aircraft.Id;

		_flightRepositoryMock
			.Setup(x => x.GetByIdAsync(existingFlight.Id))
			.ReturnsAsync(existingFlight);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(departureAirport.Id))
			.ReturnsAsync(departureAirport);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(destinationAirport.Id))
			.ReturnsAsync(destinationAirport);
		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(aircraft.Id))
			.ReturnsAsync(aircraft);

		// Act
		await _flightService.UpdateAsync(flightDto);

		// Assert
		_flightRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Flight>(f =>
			f.Id == flightDto.Id &&
			f.FlightNumber == flightDto.FlightNumber &&
			f.DistanceKm > 0 &&
			f.FuelRequiredLiters > 0 &&
			f.EstimatedFlightTimeHours > 0
		)), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_WithNonExistentFlight_ThrowsInvalidOperationException()
	{
		// Arrange
		var flightDto = GenerateFlightDto();
		flightDto.Id = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.GetByIdAsync(flightDto.Id))
			.ReturnsAsync((Flight?)null);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_flightService.UpdateAsync(flightDto));
		Assert.Contains($"Flight with ID {flightDto.Id} not found", exception.Message);
	}

	[Fact]
	public async Task UpdateAsync_WithInvalidAirport_ThrowsInvalidOperationException()
	{
		// Arrange
		var existingFlight = GenerateFlight();
		var flightDto = GenerateFlightDto();
		flightDto.Id = existingFlight.Id;

		_flightRepositoryMock
			.Setup(x => x.GetByIdAsync(existingFlight.Id))
			.ReturnsAsync(existingFlight);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(flightDto.DepartureAirportId))
			.ReturnsAsync((Airport?)null);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_flightService.UpdateAsync(flightDto));
	}

	#endregion

	#region DeleteAsync Tests

	[Fact]
	public async Task DeleteAsync_WithValidId_CallsRepositoryDelete()
	{
		// Arrange
		var flightId = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.DeleteAsync(flightId))
			.Returns(Task.CompletedTask);

		// Act
		await _flightService.DeleteAsync(flightId);

		// Assert
		_flightRepositoryMock.Verify(x => x.DeleteAsync(flightId), Times.Once);
	}

	#endregion

	#region ExistsAsync Tests

	[Fact]
	public async Task ExistsAsync_WhenFlightExists_ReturnsTrue()
	{
		// Arrange
		var flightId = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.ExistsAsync(flightId))
			.ReturnsAsync(true);

		// Act
		var result = await _flightService.ExistsAsync(flightId);

		// Assert
		Assert.True(result);
		_flightRepositoryMock.Verify(x => x.ExistsAsync(flightId), Times.Once);
	}

	[Fact]
	public async Task ExistsAsync_WhenFlightDoesNotExist_ReturnsFalse()
	{
		// Arrange
		var flightId = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.ExistsAsync(flightId))
			.ReturnsAsync(false);

		// Act
		var result = await _flightService.ExistsAsync(flightId);

		// Assert
		Assert.False(result);
		_flightRepositoryMock.Verify(x => x.ExistsAsync(flightId), Times.Once);
	}

	#endregion

	#region GetFlightReportAsync Tests

	[Fact]
	public async Task GetFlightReportAsync_WithMultipleFlights_ReturnsCorrectAggregations()
	{
		// Arrange
		var flights = GenerateFlights(3);
		_flightRepositoryMock
			.Setup(x => x.GetAllWithDetailsAsync())
			.ReturnsAsync(flights);

		// Act
		var result = await _flightService.GetFlightReportAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(flights.Count, result.TotalFlights);
		Assert.Equal(flights.Sum(f => f.DistanceKm), result.TotalDistanceKm);
		Assert.Equal(flights.Sum(f => f.FuelRequiredLiters), result.TotalFuelLiters);
		Assert.Equal(flights.Sum(f => f.EstimatedFlightTimeHours), result.TotalFlightTimeHours);
		Assert.Equal(flights.Average(f => f.DistanceKm), result.AverageDistanceKm);
		Assert.Equal(flights.Average(f => f.FuelRequiredLiters), result.AverageFuelLiters);
		Assert.Equal(flights.Count, result.Flights.Count());
	}

	[Fact]
	public async Task GetFlightReportAsync_WithNoFlights_ReturnsEmptyReport()
	{
		// Arrange
		_flightRepositoryMock
			.Setup(x => x.GetAllWithDetailsAsync())
			.ReturnsAsync(new List<Flight>());

		// Act
		var result = await _flightService.GetFlightReportAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(0, result.TotalFlights);
		Assert.Equal(0, result.TotalDistanceKm);
		Assert.Equal(0, result.TotalFuelLiters);
		Assert.Equal(0, result.AverageDistanceKm);
		Assert.Equal(0, result.AverageFuelLiters);
		Assert.Empty(result.Flights);
	}

	#endregion

	#region ValidateFlightAsync Tests

	[Fact]
	public async Task ValidateFlightAsync_WithSameDepartureAndDestination_ReturnsErrorMessage()
	{
		// Arrange
		var flightDto = GenerateFlightDto();
		flightDto.DepartureAirportId = 1;
		flightDto.DestinationAirportId = 1;

		// Act
		var result = await _flightService.ValidateFlightAsync(flightDto);

		// Assert
		Assert.NotNull(result);
		Assert.Contains("Destination airport must be different from departure airport", result);
	}

	[Fact]
	public async Task ValidateFlightAsync_WithDifferentAirports_ReturnsNull()
	{
		// Arrange
		var flightDto = GenerateFlightDto();
		flightDto.DepartureAirportId = 1;
		flightDto.DestinationAirportId = 2;

		// Act
		var result = await _flightService.ValidateFlightAsync(flightDto);

		// Assert
		Assert.Null(result);
	}

	#endregion

	#region GetFlightFormDataAsync Tests

	[Fact]
	public async Task GetFlightFormDataAsync_ReturnsFormDataWithAirportsAndAircraft()
	{
		// Arrange
		var airports = GenerateAirports(3);
		var aircraft = GenerateAircrafts(2);

		_airportRepositoryMock
			.Setup(x => x.GetAllAsync())
			.ReturnsAsync(airports);
		_aircraftRepositoryMock
			.Setup(x => x.GetAllAsync())
			.ReturnsAsync(aircraft);

		// Act
		var result = await _flightService.GetFlightFormDataAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(airports.Count, result.DepartureAirports.Count());
		Assert.Equal(airports.Count, result.DestinationAirports.Count());
		Assert.Equal(aircraft.Count, result.Aircraft.Count());
		Assert.All(result.DepartureAirports, item => Assert.NotNull(item.Display));
		Assert.All(result.DestinationAirports, item => Assert.NotNull(item.Display));
		Assert.All(result.Aircraft, item => Assert.NotNull(item.Display));
	}

	#endregion

	#region Calculation Tests

	[Fact]
	public void CalculateDistance_WithKnownCoordinates_ReturnsCorrectDistance()
	{
		// Arrange - New York to London (approximate)
		double lat1 = 40.7128;
		double lon1 = -74.0060;
		double lat2 = 51.5074;
		double lon2 = -0.1278;

		// Act
		var result = _flightService.CalculateDistance(lat1, lon1, lat2, lon2);

		// Assert
		Assert.True(result > 5500 && result < 5600); // Approximately 5570 km
	}

	[Fact]
	public void CalculateDistance_WithSameCoordinates_ReturnsZero()
	{
		// Arrange
		double lat = _faker.Address.Latitude();
		double lon = _faker.Address.Longitude();

		// Act
		var result = _flightService.CalculateDistance(lat, lon, lat, lon);

		// Assert
		Assert.Equal(0, result, 2);
	}

	[Fact]
	public void CalculateFuelRequired_WithValidInputs_ReturnsCorrectFuel()
	{
		// Arrange
		double distanceKm = 1000;
		double fuelConsumptionPerKm = 3.5;
		double takeoffEffort = 500;

		// Act
		var result = _flightService.CalculateFuelRequired(distanceKm, fuelConsumptionPerKm, takeoffEffort);

		// Assert
		Assert.Equal(4000, result); // (1000 * 3.5) + 500 = 4000
	}

	[Fact]
	public void CalculateFlightTime_WithValidInputs_ReturnsCorrectTime()
	{
		// Arrange
		double distanceKm = 800;
		double cruiseSpeed = 800;

		// Act
		var result = _flightService.CalculateFlightTime(distanceKm, cruiseSpeed);

		// Assert
		Assert.Equal(1, result); // 800 / 800 = 1 hour
	}

	#endregion

	#region Helper Methods

	private Airport GenerateAirport()
	{
		var airportFaker = new Faker<Airport>()
			.RuleFor(a => a.Id, f => f.Random.Int(1, 1000))
			.RuleFor(a => a.Code, f => f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"))
			.RuleFor(a => a.Name, f => $"{f.Address.City()} International Airport")
			.RuleFor(a => a.City, f => f.Address.City())
			.RuleFor(a => a.Country, f => f.Address.Country())
			.RuleFor(a => a.Latitude, f => f.Address.Latitude())
			.RuleFor(a => a.Longitude, f => f.Address.Longitude())
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
			.RuleFor(a => a.UpdatedAt, f => f.Date.Recent());

		return airportFaker.Generate();
	}

	private List<Airport> GenerateAirports(int count)
	{
		var airportFaker = new Faker<Airport>()
			.RuleFor(a => a.Id, f => f.IndexFaker + 1)
			.RuleFor(a => a.Code, f => f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"))
			.RuleFor(a => a.Name, f => $"{f.Address.City()} International Airport")
			.RuleFor(a => a.City, f => f.Address.City())
			.RuleFor(a => a.Country, f => f.Address.Country())
			.RuleFor(a => a.Latitude, f => f.Address.Latitude())
			.RuleFor(a => a.Longitude, f => f.Address.Longitude())
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
			.RuleFor(a => a.UpdatedAt, f => f.Date.Recent());

		return airportFaker.Generate(count);
	}

	private Aircraft GenerateAircraft()
	{
		var aircraftFaker = new Faker<Aircraft>()
			.RuleFor(a => a.Id, f => f.Random.Int(1, 1000))
			.RuleFor(a => a.Manufacturer, f => f.PickRandom("Boeing", "Airbus", "Embraer"))
			.RuleFor(a => a.Model, f => f.Random.AlphaNumeric(10))
			.RuleFor(a => a.RegistrationNumber, f => f.Random.AlphaNumeric(8))
			.RuleFor(a => a.FuelConsumptionPerKm, f => f.Random.Double(2.5, 5.5))
			.RuleFor(a => a.TakeoffFuelEffort, f => f.Random.Double(500, 2000))
			.RuleFor(a => a.MaxRangeKm, f => f.Random.Double(3000, 15000))
			.RuleFor(a => a.CruiseSpeedKmh, f => f.Random.Double(700, 950))
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(3))
			.RuleFor(a => a.UpdatedAt, f => f.Date.Recent());

		return aircraftFaker.Generate();
	}

	private List<Aircraft> GenerateAircrafts(int count)
	{
		var aircraftFaker = new Faker<Aircraft>()
			.RuleFor(a => a.Id, f => f.IndexFaker + 1)
			.RuleFor(a => a.Manufacturer, f => f.PickRandom("Boeing", "Airbus", "Embraer"))
			.RuleFor(a => a.Model, f => f.Random.AlphaNumeric(10))
			.RuleFor(a => a.RegistrationNumber, f => f.Random.AlphaNumeric(8))
			.RuleFor(a => a.FuelConsumptionPerKm, f => f.Random.Double(2.5, 5.5))
			.RuleFor(a => a.TakeoffFuelEffort, f => f.Random.Double(500, 2000))
			.RuleFor(a => a.MaxRangeKm, f => f.Random.Double(3000, 15000))
			.RuleFor(a => a.CruiseSpeedKmh, f => f.Random.Double(700, 950))
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(3))
			.RuleFor(a => a.UpdatedAt, f => f.Date.Recent());

		return aircraftFaker.Generate(count);
	}

	private Flight GenerateFlight()
	{
		var departureAirport = GenerateAirport();
		var destinationAirport = GenerateAirport();
		var aircraft = GenerateAircraft();

		var flightFaker = new Faker<Flight>()
			.RuleFor(f => f.Id, faker => faker.Random.Int(1, 1000))
			.RuleFor(f => f.FlightNumber, faker => $"{faker.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}{faker.Random.Number(1000, 9999)}")
			.RuleFor(f => f.DepartureAirportId, departureAirport.Id)
			.RuleFor(f => f.DestinationAirportId, destinationAirport.Id)
			.RuleFor(f => f.AircraftId, aircraft.Id)
			.RuleFor(f => f.DepartureAirport, departureAirport)
			.RuleFor(f => f.DestinationAirport, destinationAirport)
			.RuleFor(f => f.Aircraft, aircraft)
			.RuleFor(f => f.ScheduledDeparture, faker => faker.Date.Future())
			.RuleFor(f => f.DistanceKm, faker => faker.Random.Double(200, 3500))
			.RuleFor(f => f.EstimatedFlightTimeHours, (faker, flight) => flight.DistanceKm / 800)
			.RuleFor(f => f.FuelRequiredLiters, (faker, flight) => (flight.DistanceKm * aircraft.FuelConsumptionPerKm) + aircraft.TakeoffFuelEffort)
			.RuleFor(f => f.Status, faker => faker.PickRandom<FlightStatus>())
			.RuleFor(f => f.ActualDeparture, faker => faker.Random.Bool() ? faker.Date.Recent() : null)
			.RuleFor(f => f.ActualArrival, faker => faker.Random.Bool() ? faker.Date.Recent() : null)
			.RuleFor(f => f.CreatedAt, faker => faker.Date.Past(2))
			.RuleFor(f => f.UpdatedAt, faker => faker.Date.Recent());

		return flightFaker.Generate();
	}

	private List<Flight> GenerateFlights(int count)
	{
		var flights = new List<Flight>();
		for (int i = 0; i < count; i++)
		{
			flights.Add(GenerateFlight());
		}
		return flights;
	}

	private FlightDto GenerateFlightDto()
	{
		var flightDtoFaker = new Faker<FlightDto>()
			.RuleFor(f => f.Id, faker => faker.Random.Int(1, 1000))
			.RuleFor(f => f.FlightNumber, faker => $"{faker.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}{faker.Random.Number(1000, 9999)}")
			.RuleFor(f => f.DepartureAirportId, faker => faker.Random.Int(1, 100))
			.RuleFor(f => f.DestinationAirportId, faker => faker.Random.Int(101, 200))
			.RuleFor(f => f.AircraftId, faker => faker.Random.Int(1, 50))
			.RuleFor(f => f.ScheduledDeparture, faker => faker.Date.Future())
			.RuleFor(f => f.Status, faker => faker.PickRandom<FlightStatus>())
			.RuleFor(f => f.CreatedAt, faker => faker.Date.Past(2))
			.RuleFor(f => f.UpdatedAt, faker => faker.Date.Recent());

		return flightDtoFaker.Generate();
	}

	#endregion
}
