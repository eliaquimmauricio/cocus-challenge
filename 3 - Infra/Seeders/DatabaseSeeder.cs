using Bogus;
using Cocus.Domain.Entities;
using Cocus.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Cocus.Infra.Data.Seeders;

public static class DatabaseSeeder
{
	public static void Seed(ApplicationDbContext context)
	{
		Randomizer.Seed = new Random(123456);

		context.Flights.RemoveRange(context.Flights);
		context.Aircraft.RemoveRange(context.Aircraft);
		context.Airports.RemoveRange(context.Airports);
		context.SaveChanges();

		List<Airport> airports = SeedAirports();
		context.Airports.AddRange(airports);
		context.SaveChanges();

		List<Aircraft> aircraft = SeedAircraft();
		context.Aircraft.AddRange(aircraft);
		context.SaveChanges();

		List<Flight> flights = SeedFlights(airports, aircraft);
		context.Flights.AddRange(flights);
		context.SaveChanges();
	}

	private static List<Airport> SeedAirports()
	{
		Faker<Airport> airportFaker = new Faker<Airport>("en_US")
			.RuleFor(a => a.Code, f => f.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"))
			.RuleFor(a => a.Name, f => $"{f.Address.City()} International Airport")
			.RuleFor(a => a.City, f => f.Address.City())
			.RuleFor(a => a.Country, f => f.Address.Country())
			.RuleFor(a => a.Latitude, f => f.Address.Latitude())
			.RuleFor(a => a.Longitude, f => f.Address.Longitude())
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
			.RuleFor(a => a.UpdatedAt, (f, a) => f.Date.Between(a.CreatedAt, DateTime.Now));

		return airportFaker.Generate(5);
	}

	private static List<Aircraft> SeedAircraft()
	{
		string[] manufacturers = { "Boeing", "Airbus", "Embraer", "Bombardier", "ATR" };

		Dictionary<string, string[]> models = new Dictionary<string, string[]>
		{
			{ "Boeing", new[] { "737-800", "737 MAX 8", "777-300ER", "787-9 Dreamliner" } },
			{ "Airbus", new[] { "A320neo", "A321", "A330-200", "A350-900" } },
			{ "Embraer", new[] { "E195-E2", "E190", "E175", "E145" } },
			{ "Bombardier", new[] { "CRJ-900", "CRJ-700", "Q400" } },
			{ "ATR", new[] { "ATR 72-600", "ATR 42-500" } }
		};

		Faker<Aircraft> aircraftFaker = new Faker<Aircraft>("en_US")
			.RuleFor(a => a.Manufacturer, f => f.PickRandom(manufacturers))
			.RuleFor(a => a.Model, (f, a) => f.PickRandom(models[a.Manufacturer]))
			.RuleFor(a => a.RegistrationNumber, f => $"N{f.Random.Number(100, 999)}{f.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}")
			.RuleFor(a => a.FuelConsumptionPerKm, f => f.Random.Double(2.5, 5.5))
			.RuleFor(a => a.TakeoffFuelEffort, f => f.Random.Double(500, 2000))
			.RuleFor(a => a.MaxRangeKm, f => f.Random.Double(3000, 15000))
			.RuleFor(a => a.CruiseSpeedKmh, f => f.Random.Double(700, 950))
			.RuleFor(a => a.CreatedAt, f => f.Date.Past(3))
			.RuleFor(a => a.UpdatedAt, (f, a) => f.Date.Between(a.CreatedAt, DateTime.Now));

		return aircraftFaker.Generate(5);
	}

	private static List<Flight> SeedFlights(List<Airport> airports, List<Aircraft> aircraft)
	{
		FlightStatus[] flightStatuses = Enum.GetValues<FlightStatus>();

		Faker<Flight> flightFaker = new Faker<Flight>("en_US")
			.RuleFor(f => f.FlightNumber, faker => $"{faker.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}{faker.Random.Number(1000, 9999)}")
			.RuleFor(f => f.DepartureAirportId, faker => faker.PickRandom(airports).Id)
			.RuleFor(f => f.DestinationAirportId, (faker, flight) =>
			{
				Airport destination = faker.PickRandom(airports);
				while (destination.Id == flight.DepartureAirportId)
				{
					destination = faker.PickRandom(airports);
				}
				return destination.Id;
			})
			.RuleFor(f => f.AircraftId, faker => faker.PickRandom(aircraft).Id)
			.RuleFor(f => f.ScheduledDeparture, faker => faker.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(60)))
			.RuleFor(f => f.DistanceKm, faker => faker.Random.Double(200, 3500))
			.RuleFor(f => f.EstimatedFlightTimeHours, (faker, flight) => flight.DistanceKm / 800)
			.RuleFor(f => f.FuelRequiredLiters, (faker, flight) =>
			{
				Aircraft selectedAircraft = aircraft.First(a => a.Id == flight.AircraftId);
				return (flight.DistanceKm * selectedAircraft.FuelConsumptionPerKm) + selectedAircraft.TakeoffFuelEffort;
			})
			.RuleFor(f => f.Status, faker => faker.PickRandom(flightStatuses))
			.RuleFor(f => f.ActualDeparture, (faker, flight) =>
			{
				if (flight.Status >= FlightStatus.Departed && flight.ScheduledDeparture < DateTime.Now)
					return faker.Date.Between(flight.ScheduledDeparture.AddMinutes(-30), flight.ScheduledDeparture.AddMinutes(60));
				return null;
			})
			.RuleFor(f => f.ActualArrival, (faker, flight) =>
			{
				if (flight.Status >= FlightStatus.Landed && flight.ActualDeparture.HasValue)
					return flight.ActualDeparture.Value.AddHours(flight.EstimatedFlightTimeHours).AddMinutes(faker.Random.Int(-20, 40));
				return null;
			})
			.RuleFor(f => f.CreatedAt, faker => faker.Date.Past(2))
			.RuleFor(f => f.UpdatedAt, (faker, flight) => faker.Date.Between(flight.CreatedAt, DateTime.Now));

		return flightFaker.Generate(5);
	}
}
