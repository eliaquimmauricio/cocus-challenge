using Bogus;
using Cocus.Domain.DTOs;
using Cocus.Domain.Entities;
using Cocus.Domain.Interfaces.Repositories;
using Cocus.Domain.Services;
using Moq;

namespace Cocus.Tests.Services;

public class AirportServiceTests
{
	private readonly Mock<IAirportRepository> _airportRepositoryMock;
	private readonly Mock<IFlightRepository> _flightRepositoryMock;
	private readonly AirportService _airportService;
	private readonly Faker _faker;

	public AirportServiceTests()
	{
		_airportRepositoryMock = new Mock<IAirportRepository>();
		_flightRepositoryMock = new Mock<IFlightRepository>();
		_airportService = new AirportService(
			_airportRepositoryMock.Object,
			_flightRepositoryMock.Object);
		_faker = new Faker();
	}

	#region GetByIdAsync Tests

	[Fact]
	public async Task GetByIdAsync_WhenAirportExists_ReturnsAirportDto()
	{
		// Arrange
		var airport = GenerateAirport();
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(airport.Id))
			.ReturnsAsync(airport);

		// Act
		var result = await _airportService.GetByIdAsync(airport.Id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(airport.Id, result.Id);
		Assert.Equal(airport.Code, result.Code);
		Assert.Equal(airport.Name, result.Name);
		Assert.Equal(airport.City, result.City);
		Assert.Equal(airport.Country, result.Country);
		Assert.Equal(airport.Latitude, result.Latitude);
		Assert.Equal(airport.Longitude, result.Longitude);
		_airportRepositoryMock.Verify(x => x.GetByIdAsync(airport.Id), Times.Once);
	}

	[Fact]
	public async Task GetByIdAsync_WhenAirportDoesNotExist_ReturnsNull()
	{
		// Arrange
		var airportId = _faker.Random.Int(1, 1000);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(airportId))
			.ReturnsAsync((Airport?)null);

		// Act
		var result = await _airportService.GetByIdAsync(airportId);

		// Assert
		Assert.Null(result);
		_airportRepositoryMock.Verify(x => x.GetByIdAsync(airportId), Times.Once);
	}

	#endregion

	#region GetAllAsync Tests

	[Fact]
	public async Task GetAllAsync_WhenAirportsExist_ReturnsAllAirportDtos()
	{
		// Arrange
		var airports = GenerateAirports(5);
		_airportRepositoryMock
			.Setup(x => x.GetAllAsync())
			.ReturnsAsync(airports);

		// Act
		var result = await _airportService.GetAllAsync();

		// Assert
		var resultList = result.ToList();
		Assert.NotNull(result);
		Assert.Equal(airports.Count, resultList.Count);
		Assert.All(resultList, dto => Assert.NotNull(dto.Code));
		Assert.All(resultList, dto => Assert.NotNull(dto.Name));
		_airportRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
	}

	[Fact]
	public async Task GetAllAsync_WhenNoAirportsExist_ReturnsEmptyCollection()
	{
		// Arrange
		_airportRepositoryMock
			.Setup(x => x.GetAllAsync())
			.ReturnsAsync(new List<Airport>());

		// Act
		var result = await _airportService.GetAllAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
		_airportRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
	}

	#endregion

	#region GetByCodeAsync Tests

	[Fact]
	public async Task GetByCodeAsync_WhenAirportExists_ReturnsAirportDto()
	{
		// Arrange
		var airport = GenerateAirport();
		_airportRepositoryMock
			.Setup(x => x.GetByCodeAsync(airport.Code))
			.ReturnsAsync(airport);

		// Act
		var result = await _airportService.GetByCodeAsync(airport.Code);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(airport.Code, result.Code);
		Assert.Equal(airport.Name, result.Name);
		_airportRepositoryMock.Verify(x => x.GetByCodeAsync(airport.Code), Times.Once);
	}

	[Fact]
	public async Task GetByCodeAsync_WhenAirportDoesNotExist_ReturnsNull()
	{
		// Arrange
		var code = _faker.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
		_airportRepositoryMock
			.Setup(x => x.GetByCodeAsync(code))
			.ReturnsAsync((Airport?)null);

		// Act
		var result = await _airportService.GetByCodeAsync(code);

		// Assert
		Assert.Null(result);
		_airportRepositoryMock.Verify(x => x.GetByCodeAsync(code), Times.Once);
	}

	#endregion

	#region CreateAsync Tests

	[Fact]
	public async Task CreateAsync_WithValidData_CreatesAirport()
	{
		// Arrange
		var airportDto = GenerateAirportDto();
		var createdAirport = new Airport
		{
			Id = _faker.Random.Int(1, 1000),
			Code = airportDto.Code,
			Name = airportDto.Name,
			City = airportDto.City,
			Country = airportDto.Country,
			Latitude = airportDto.Latitude,
			Longitude = airportDto.Longitude,
			CreatedAt = DateTime.UtcNow
		};

		_airportRepositoryMock
			.Setup(x => x.AddAsync(It.IsAny<Airport>()))
			.ReturnsAsync(createdAirport);

		// Act
		var result = await _airportService.CreateAsync(airportDto);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(airportDto.Code, result.Code);
		Assert.Equal(airportDto.Name, result.Name);
		Assert.Equal(airportDto.City, result.City);
		Assert.Equal(airportDto.Country, result.Country);
		Assert.Equal(airportDto.Latitude, result.Latitude);
		Assert.Equal(airportDto.Longitude, result.Longitude);
		_airportRepositoryMock.Verify(x => x.AddAsync(It.Is<Airport>(a =>
			a.Code == airportDto.Code &&
			a.Name == airportDto.Name &&
			a.City == airportDto.City
		)), Times.Once);
	}

	#endregion

	#region UpdateAsync Tests

	[Fact]
	public async Task UpdateAsync_WithValidData_UpdatesAirport()
	{
		// Arrange
		var existingAirport = GenerateAirport();
		var airportDto = GenerateAirportDto();
		airportDto.Id = existingAirport.Id;

		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(existingAirport.Id))
			.ReturnsAsync(existingAirport);
		_airportRepositoryMock
			.Setup(x => x.UpdateAsync(It.IsAny<Airport>()))
			.Returns(Task.CompletedTask);

		// Act
		await _airportService.UpdateAsync(airportDto);

		// Assert
		_airportRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Airport>(a =>
			a.Id == airportDto.Id &&
			a.Code == airportDto.Code &&
			a.Name == airportDto.Name &&
			a.City == airportDto.City &&
			a.Country == airportDto.Country &&
			a.Latitude == airportDto.Latitude &&
			a.Longitude == airportDto.Longitude
		)), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_WithNonExistentAirport_ThrowsInvalidOperationException()
	{
		// Arrange
		var airportDto = GenerateAirportDto();
		airportDto.Id = _faker.Random.Int(1, 1000);
		_airportRepositoryMock
			.Setup(x => x.GetByIdAsync(airportDto.Id))
			.ReturnsAsync((Airport?)null);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_airportService.UpdateAsync(airportDto));
		Assert.Contains($"Airport with ID {airportDto.Id} not found", exception.Message);
	}

	#endregion

	#region DeleteAsync Tests

	[Fact]
	public async Task DeleteAsync_WithNoAssociatedFlights_DeletesAirport()
	{
		// Arrange
		var airportId = _faker.Random.Int(1, 1000);
		_flightRepositoryMock
			.Setup(x => x.GetFlightsByAirportAsync(airportId))
			.ReturnsAsync(new List<Flight>());
		_airportRepositoryMock
			.Setup(x => x.DeleteAsync(airportId))
			.Returns(Task.CompletedTask);

		// Act
		await _airportService.DeleteAsync(airportId);

		// Assert
		_airportRepositoryMock.Verify(x => x.DeleteAsync(airportId), Times.Once);
		_flightRepositoryMock.Verify(x => x.GetFlightsByAirportAsync(airportId), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_WithAssociatedFlights_ThrowsInvalidOperationException()
	{
		// Arrange
		var airportId = _faker.Random.Int(1, 1000);
		var flights = new List<Flight>
		{
			new Flight { Id = 1, DepartureAirportId = airportId },
			new Flight { Id = 2, DepartureAirportId = airportId },
			new Flight { Id = 3, DestinationAirportId = airportId }
		};

		_flightRepositoryMock
			.Setup(x => x.GetFlightsByAirportAsync(airportId))
			.ReturnsAsync(flights);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
			_airportService.DeleteAsync(airportId));
		Assert.Contains("Cannot delete this airport", exception.Message);
		Assert.Contains("3 associated flight(s)", exception.Message);
		_airportRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
	}

	#endregion

	#region ExistsAsync Tests

	[Fact]
	public async Task ExistsAsync_WhenAirportExists_ReturnsTrue()
	{
		// Arrange
		var airportId = _faker.Random.Int(1, 1000);
		_airportRepositoryMock
			.Setup(x => x.ExistsAsync(airportId))
			.ReturnsAsync(true);

		// Act
		var result = await _airportService.ExistsAsync(airportId);

		// Assert
		Assert.True(result);
		_airportRepositoryMock.Verify(x => x.ExistsAsync(airportId), Times.Once);
	}

	[Fact]
	public async Task ExistsAsync_WhenAirportDoesNotExist_ReturnsFalse()
	{
		// Arrange
		var airportId = _faker.Random.Int(1, 1000);
		_airportRepositoryMock
			.Setup(x => x.ExistsAsync(airportId))
			.ReturnsAsync(false);

		// Act
		var result = await _airportService.ExistsAsync(airportId);

		// Assert
		Assert.False(result);
		_airportRepositoryMock.Verify(x => x.ExistsAsync(airportId), Times.Once);
	}

	#endregion

	#region SearchAsync Tests

	[Fact]
	public async Task SearchAsync_WithMatchingResults_ReturnsMatchingAirports()
	{
		// Arrange
		var searchTerm = "New York";
		var airports = GenerateAirports(3);
		_airportRepositoryMock
			.Setup(x => x.SearchByNameOrCityAsync(searchTerm))
			.ReturnsAsync(airports);

		// Act
		var result = await _airportService.SearchAsync(searchTerm);

		// Assert
		var resultList = result.ToList();
		Assert.NotNull(result);
		Assert.Equal(airports.Count, resultList.Count);
		_airportRepositoryMock.Verify(x => x.SearchByNameOrCityAsync(searchTerm), Times.Once);
	}

	[Fact]
	public async Task SearchAsync_WithNoMatchingResults_ReturnsEmptyCollection()
	{
		// Arrange
		var searchTerm = "NonExistent";
		_airportRepositoryMock
			.Setup(x => x.SearchByNameOrCityAsync(searchTerm))
			.ReturnsAsync(new List<Airport>());

		// Act
		var result = await _airportService.SearchAsync(searchTerm);

		// Assert
		Assert.NotNull(result);
		Assert.Empty(result);
		_airportRepositoryMock.Verify(x => x.SearchByNameOrCityAsync(searchTerm), Times.Once);
	}

	#endregion

	#region ValidateAirportAsync Tests

	[Fact]
	public async Task ValidateAirportAsync_WithDuplicateCode_ReturnsErrorMessage()
	{
		// Arrange
		var existingAirport = GenerateAirport();
		var airportDto = GenerateAirportDto();
		airportDto.Id = existingAirport.Id + 1;
		airportDto.Code = existingAirport.Code;

		_airportRepositoryMock
			.Setup(x => x.GetByCodeAsync(airportDto.Code))
			.ReturnsAsync(existingAirport);

		// Act
		var result = await _airportService.ValidateAirportAsync(airportDto);

		// Assert
		Assert.NotNull(result);
		Assert.Contains($"An airport with code '{airportDto.Code}' already exists", result);
	}

	[Fact]
	public async Task ValidateAirportAsync_WithSameAirportCode_ReturnsNull()
	{
		// Arrange
		var existingAirport = GenerateAirport();
		var airportDto = GenerateAirportDto();
		airportDto.Id = existingAirport.Id;
		airportDto.Code = existingAirport.Code;

		_airportRepositoryMock
			.Setup(x => x.GetByCodeAsync(airportDto.Code))
			.ReturnsAsync(existingAirport);

		// Act
		var result = await _airportService.ValidateAirportAsync(airportDto);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task ValidateAirportAsync_WithValidData_ReturnsNull()
	{
		// Arrange
		var airportDto = GenerateAirportDto();

		_airportRepositoryMock
			.Setup(x => x.GetByCodeAsync(airportDto.Code))
			.ReturnsAsync((Airport?)null);

		// Act
		var result = await _airportService.ValidateAirportAsync(airportDto);

		// Assert
		Assert.Null(result);
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

	private AirportDto GenerateAirportDto()
	{
		var airportDtoFaker = new Faker<AirportDto>()
			.RuleFor(a => a.Id, f => f.Random.Int(1, 1000))
			.RuleFor(a => a.Code, f => f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"))
			.RuleFor(a => a.Name, f => $"{f.Address.City()} International Airport")
			.RuleFor(a => a.City, f => f.Address.City())
			.RuleFor(a => a.Country, f => f.Address.Country())
			.RuleFor(a => a.Latitude, f => f.Address.Latitude())
			.RuleFor(a => a.Longitude, f => f.Address.Longitude())
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
			.RuleFor(a => a.UpdatedAt, f => f.Date.Recent());

		return airportDtoFaker.Generate();
	}

	#endregion
}
