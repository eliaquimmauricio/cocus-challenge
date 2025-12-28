using Bogus;
using Cocus.Domain.DTOs;
using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Domain.Services;
using Moq;

namespace Cocus.Tests.Services;

public class AircraftServiceTests
{
	private readonly Mock<IAircraftRepository> _aircraftRepositoryMock;
	private readonly Mock<IFlightRepository> _flightRepositoryMock;
	private readonly AircraftService _aircraftService;
	private readonly Faker _faker;

	public AircraftServiceTests()
	{
		_aircraftRepositoryMock = new Mock<IAircraftRepository>();
		_flightRepositoryMock = new Mock<IFlightRepository>();
		_aircraftService = new AircraftService(
			_aircraftRepositoryMock.Object,
			_flightRepositoryMock.Object);
		_faker = new Faker();
	}

	#region GetByIdAsync Tests

	[Fact]
	public async Task GetByIdAsync_WhenAircraftExists_ReturnsAircraftDto()
	{
		// Arrange
		var aircraft = GenerateAircraft();
		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(aircraft.Id))
			.ReturnsAsync(aircraft);

		// Act
		var result = await _aircraftService.GetByIdAsync(aircraft.Id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(aircraft.Id, result.Id);
		Assert.Equal(aircraft.Model, result.Model);
		Assert.Equal(aircraft.Manufacturer, result.Manufacturer);
		Assert.Equal(aircraft.RegistrationNumber, result.RegistrationNumber);
		Assert.Equal(aircraft.FuelConsumptionPerKm, result.FuelConsumptionPerKm);
		Assert.Equal(aircraft.CruiseSpeedKmh, result.CruiseSpeedKmh);
		_aircraftRepositoryMock.Verify(x => x.GetByIdAsync(aircraft.Id), Times.Once);
	}

	[Fact]
	public async Task GetByIdAsync_WhenAircraftDoesNotExist_ReturnsNull()
	{
		// Arrange
		var aircraftId = _faker.Random.Int(1, 1000);
		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(aircraftId))
			.ReturnsAsync((Aircraft?)null);

		// Act
		var result = await _aircraftService.GetByIdAsync(aircraftId);

		// Assert
		Assert.Null(result);
		_aircraftRepositoryMock.Verify(x => x.GetByIdAsync(aircraftId), Times.Once);
	}

	#endregion

	#region GetAllAsync Tests

	[Fact]
	public async Task GetAllAsync_WhenAircraftExist_ReturnsAllAircraftDtos()
	{
		// Arrange
		var aircraft = GenerateAircrafts(5);
		_aircraftRepositoryMock
			.Setup(x => x.GetAllAsync())
			.ReturnsAsync(aircraft);

		// Act
		var result = await _aircraftService.GetAllAsync();

		// Assert
		var resultList = result.ToList();
		Assert.NotNull(result);
		Assert.Equal(aircraft.Count, resultList.Count);
		Assert.All(resultList, dto => Assert.NotNull(dto.Model));
		Assert.All(resultList, dto => Assert.NotNull(dto.RegistrationNumber));
		_aircraftRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
	}

	[Fact]
	public async Task GetAllAsync_WhenNoAircraftExist_ReturnsEmptyCollection()
	{
		// Arrange
		_aircraftRepositoryMock
			.Setup(x => x.GetAllAsync())
			.ReturnsAsync(new List<Aircraft>());

		// Act
		var result = await _aircraftService.GetAllAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
		_aircraftRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
	}

	#endregion

	#region GetByRegistrationNumberAsync Tests

	[Fact]
	public async Task GetByRegistrationNumberAsync_WhenAircraftExists_ReturnsAircraftDto()
	{
		// Arrange
		var aircraft = GenerateAircraft();
		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraft.RegistrationNumber))
			.ReturnsAsync(aircraft);

		// Act
		var result = await _aircraftService.GetByRegistrationNumberAsync(aircraft.RegistrationNumber);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(aircraft.RegistrationNumber, result.RegistrationNumber);
		Assert.Equal(aircraft.Model, result.Model);
		_aircraftRepositoryMock.Verify(x => x.GetByRegistrationNumberAsync(aircraft.RegistrationNumber), Times.Once);
	}

	[Fact]
	public async Task GetByRegistrationNumberAsync_WhenAircraftDoesNotExist_ReturnsNull()
	{
		// Arrange
		var registrationNumber = _faker.Random.AlphaNumeric(8);
		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(registrationNumber))
			.ReturnsAsync((Aircraft?)null);

		// Act
		var result = await _aircraftService.GetByRegistrationNumberAsync(registrationNumber);

		// Assert
		Assert.Null(result);
		_aircraftRepositoryMock.Verify(x => x.GetByRegistrationNumberAsync(registrationNumber), Times.Once);
	}

	#endregion

	#region CreateAsync Tests

	[Fact]
	public async Task CreateAsync_WithValidData_CreatesAircraft()
	{
		// Arrange
		var aircraftDto = GenerateAircraftDto();
		var createdAircraft = new Aircraft
		{
			Id = _faker.Random.Int(1, 1000),
			Model = aircraftDto.Model,
			Manufacturer = aircraftDto.Manufacturer,
			RegistrationNumber = aircraftDto.RegistrationNumber,
			FuelConsumptionPerKm = aircraftDto.FuelConsumptionPerKm,
			TakeoffFuelEffort = aircraftDto.TakeoffFuelEffort,
			MaxRangeKm = aircraftDto.MaxRangeKm,
			CruiseSpeedKmh = aircraftDto.CruiseSpeedKmh,
			CreatedAt = DateTime.UtcNow
		};

		_aircraftRepositoryMock
			.Setup(x => x.AddAsync(It.IsAny<Aircraft>()))
			.ReturnsAsync(createdAircraft);

		// Act
		var result = await _aircraftService.CreateAsync(aircraftDto);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(aircraftDto.Model, result.Model);
		Assert.Equal(aircraftDto.Manufacturer, result.Manufacturer);
		Assert.Equal(aircraftDto.RegistrationNumber, result.RegistrationNumber);
		Assert.Equal(aircraftDto.FuelConsumptionPerKm, result.FuelConsumptionPerKm);
		_aircraftRepositoryMock.Verify(x => x.AddAsync(It.Is<Aircraft>(a =>
			a.Model == aircraftDto.Model &&
			a.Manufacturer == aircraftDto.Manufacturer &&
			a.RegistrationNumber == aircraftDto.RegistrationNumber
		)), Times.Once);
	}

	#endregion

	#region UpdateAsync Tests

	[Fact]
	public async Task UpdateAsync_WithValidData_UpdatesAircraft()
	{
		// Arrange
		var existingAircraft = GenerateAircraft();
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.Id = existingAircraft.Id;

		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(existingAircraft.Id))
			.ReturnsAsync(existingAircraft);
		_aircraftRepositoryMock
			.Setup(x => x.UpdateAsync(It.IsAny<Aircraft>()))
			.Returns(Task.CompletedTask);

		// Act
		await _aircraftService.UpdateAsync(aircraftDto);

		// Assert
		_aircraftRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Aircraft>(a =>
			a.Id == aircraftDto.Id &&
			a.Model == aircraftDto.Model &&
			a.Manufacturer == aircraftDto.Manufacturer &&
			a.RegistrationNumber == aircraftDto.RegistrationNumber &&
			a.FuelConsumptionPerKm == aircraftDto.FuelConsumptionPerKm &&
			a.TakeoffFuelEffort == aircraftDto.TakeoffFuelEffort &&
			a.MaxRangeKm == aircraftDto.MaxRangeKm &&
			a.CruiseSpeedKmh == aircraftDto.CruiseSpeedKmh
		)), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_WithNonExistentAircraft_ThrowsInvalidOperationException()
	{
		// Arrange
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.Id = _faker.Random.Int(1, 1000);
		_aircraftRepositoryMock
			.Setup(x => x.GetByIdAsync(aircraftDto.Id))
			.ReturnsAsync((Aircraft?)null);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_aircraftService.UpdateAsync(aircraftDto));
		Assert.Contains($"Aircraft with ID {aircraftDto.Id} not found", exception.Message);
	}

	#endregion

	#region DeleteAsync Tests

	[Fact]
	public async Task DeleteAsync_WithNoAssociatedFlights_DeletesAircraft()
	{
		// Arrange
		var aircraftId = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.GetFlightsByAircraftAsync(aircraftId))
			.ReturnsAsync(new List<Flight>());
		_aircraftRepositoryMock
			.Setup(x => x.DeleteAsync(aircraftId))
			.Returns(Task.CompletedTask);

		// Act
		await _aircraftService.DeleteAsync(aircraftId);

		// Assert
		_aircraftRepositoryMock.Verify(x => x.DeleteAsync(aircraftId), Times.Once);
		_flightRepositoryMock.Verify(x => x.GetFlightsByAircraftAsync(aircraftId), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_WithAssociatedFlights_ThrowsInvalidOperationException()
	{
		// Arrange
		var aircraftId = _faker.Random.Int(1, 1000);
		var flights = new List<Flight>
		{
			new Flight { Id = 1, AircraftId = aircraftId },
			new Flight { Id = 2, AircraftId = aircraftId }
		};

		_flightRepositoryMock
			.Setup(x => x.GetFlightsByAircraftAsync(aircraftId))
			.ReturnsAsync(flights);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_aircraftService.DeleteAsync(aircraftId));
		Assert.Contains("Cannot delete this aircraft", exception.Message);
		Assert.Contains("2 associated flight(s)", exception.Message);
		_aircraftRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
	}

	#endregion

	#region ExistsAsync Tests

	[Fact]
	public async Task ExistsAsync_WhenAircraftExists_ReturnsTrue()
	{
		// Arrange
		var aircraftId = _faker.Random.Int(1, 1000);
		_aircraftRepositoryMock
			.Setup(x => x.ExistsAsync(aircraftId))
			.ReturnsAsync(true);

		// Act
		var result = await _aircraftService.ExistsAsync(aircraftId);

		// Assert
		Assert.True(result);
		_aircraftRepositoryMock.Verify(x => x.ExistsAsync(aircraftId), Times.Once);
	}

	[Fact]
	public async Task ExistsAsync_WhenAircraftDoesNotExist_ReturnsFalse()
	{
		// Arrange
		var aircraftId = _faker.Random.Int(1, 1000);
		_aircraftRepositoryMock
			.Setup(x => x.ExistsAsync(aircraftId))
			.ReturnsAsync(false);

		// Act
		var result = await _aircraftService.ExistsAsync(aircraftId);

		// Assert
		Assert.False(result);
		_aircraftRepositoryMock.Verify(x => x.ExistsAsync(aircraftId), Times.Once);
	}

	#endregion

	#region GetAvailableAircraftAsync Tests

	[Fact]
	public async Task GetAvailableAircraftAsync_ReturnsAvailableAircraft()
	{
		// Arrange
		var aircraft = GenerateAircrafts(3);
		_aircraftRepositoryMock
			.Setup(x => x.GetAvailableAircraftAsync())
			.ReturnsAsync(aircraft);

		// Act
		var result = await _aircraftService.GetAvailableAircraftAsync();

		// Assert
		var resultList = result.ToList();
		Assert.NotNull(result);
		Assert.Equal(aircraft.Count, resultList.Count);
		_aircraftRepositoryMock.Verify(x => x.GetAvailableAircraftAsync(), Times.Once);
	}

	#endregion

	#region ValidateAircraftAsync Tests

	[Fact]
	public async Task ValidateAircraftAsync_WithDuplicateRegistrationNumber_ReturnsErrorMessage()
	{
		// Arrange
		var existingAircraft = GenerateAircraft();
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.Id = existingAircraft.Id + 1;
		aircraftDto.RegistrationNumber = existingAircraft.RegistrationNumber;

		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber))
			.ReturnsAsync(existingAircraft);

		// Act
		var result = await _aircraftService.ValidateAircraftAsync(aircraftDto);

		// Assert
		Assert.NotNull(result);
		Assert.Contains($"An aircraft with registration number '{aircraftDto.RegistrationNumber}' already exists", result);
	}

	[Fact]
	public async Task ValidateAircraftAsync_WithSameAircraftRegistrationNumber_ReturnsNull()
	{
		// Arrange
		var existingAircraft = GenerateAircraft();
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.Id = existingAircraft.Id;
		aircraftDto.RegistrationNumber = existingAircraft.RegistrationNumber;

		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber))
			.ReturnsAsync(existingAircraft);

		// Act
		var result = await _aircraftService.ValidateAircraftAsync(aircraftDto);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task ValidateAircraftAsync_WithInvalidFuelConsumption_ReturnsErrorMessage()
	{
		// Arrange
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.FuelConsumptionPerKm = 0;

		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber))
			.ReturnsAsync((Aircraft?)null);

		// Act
		var result = await _aircraftService.ValidateAircraftAsync(aircraftDto);

		// Assert
		Assert.NotNull(result);
		Assert.Contains("Fuel consumption per km must be greater than zero", result);
	}

	[Fact]
	public async Task ValidateAircraftAsync_WithInvalidMaxRange_ReturnsErrorMessage()
	{
		// Arrange
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.MaxRangeKm = -100;

		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber))
			.ReturnsAsync((Aircraft?)null);

		// Act
		var result = await _aircraftService.ValidateAircraftAsync(aircraftDto);

		// Assert
		Assert.NotNull(result);
		Assert.Contains("Max range must be greater than zero", result);
	}

	[Fact]
	public async Task ValidateAircraftAsync_WithInvalidCruiseSpeed_ReturnsErrorMessage()
	{
		// Arrange
		var aircraftDto = GenerateAircraftDto();
		aircraftDto.CruiseSpeedKmh = 0;

		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber))
			.ReturnsAsync((Aircraft?)null);

		// Act
		var result = await _aircraftService.ValidateAircraftAsync(aircraftDto);

		// Assert
		Assert.NotNull(result);
		Assert.Contains("Cruise speed must be greater than zero", result);
	}

	[Fact]
	public async Task ValidateAircraftAsync_WithValidData_ReturnsNull()
	{
		// Arrange
		var aircraftDto = GenerateAircraftDto();

		_aircraftRepositoryMock
			.Setup(x => x.GetByRegistrationNumberAsync(aircraftDto.RegistrationNumber))
			.ReturnsAsync((Aircraft?)null);

		// Act
		var result = await _aircraftService.ValidateAircraftAsync(aircraftDto);

		// Assert
		Assert.Null(result);
	}

	#endregion

	#region Helper Methods

	private Aircraft GenerateAircraft()
	{
		var aircraftFaker = new Faker<Aircraft>()
			.RuleFor(a => a.Id, f => f.Random.Int(1, 1000))
			.RuleFor(a => a.Manufacturer, f => f.PickRandom("Boeing", "Airbus", "Embraer", "Bombardier"))
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
			.RuleFor(a => a.Manufacturer, f => f.PickRandom("Boeing", "Airbus", "Embraer", "Bombardier"))
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

	private AircraftDto GenerateAircraftDto()
	{
		var aircraftDtoFaker = new Faker<AircraftDto>()
			.RuleFor(a => a.Id, f => f.Random.Int(1, 1000))
			.RuleFor(a => a.Manufacturer, f => f.PickRandom("Boeing", "Airbus", "Embraer", "Bombardier"))
			.RuleFor(a => a.Model, f => f.Random.AlphaNumeric(10))
			.RuleFor(a => a.RegistrationNumber, f => f.Random.AlphaNumeric(8))
			.RuleFor(a => a.FuelConsumptionPerKm, f => f.Random.Double(2.5, 5.5))
			.RuleFor(a => a.TakeoffFuelEffort, f => f.Random.Double(500, 2000))
			.RuleFor(a => a.MaxRangeKm, f => f.Random.Double(3000, 15000))
			.RuleFor(a => a.CruiseSpeedKmh, f => f.Random.Double(700, 950))
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(3))
			.RuleFor(a => a.UpdatedAt, f => f.Date.Recent());

		return aircraftDtoFaker.Generate();
	}

	#endregion
}
