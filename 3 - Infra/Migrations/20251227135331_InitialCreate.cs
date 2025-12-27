using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cocus.Infra.Data.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Aircraft",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					RegistrationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
					FuelConsumptionPerKm = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					TakeoffFuelEffort = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					MaxRangeKm = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					CruiseSpeedKmh = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Aircraft", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Airports",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
					Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Latitude = table.Column<double>(type: "float(10)", precision: 10, scale: 7, nullable: false),
					Longitude = table.Column<double>(type: "float(10)", precision: 10, scale: 7, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Airports", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Customers",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
					Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Customers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Products",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
					Stock = table.Column<int>(type: "int", nullable: false),
					IsActive = table.Column<bool>(type: "bit", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Products", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Flights",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					FlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
					DepartureAirportId = table.Column<int>(type: "int", nullable: false),
					DestinationAirportId = table.Column<int>(type: "int", nullable: false),
					AircraftId = table.Column<int>(type: "int", nullable: false),
					ScheduledDeparture = table.Column<DateTime>(type: "datetime2", nullable: false),
					ActualDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
					ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
					DistanceKm = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					EstimatedFlightTimeHours = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					FuelRequiredLiters = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
					Status = table.Column<int>(type: "int", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Flights", x => x.Id);
					table.ForeignKey(
						name: "FK_Flights_Aircraft_AircraftId",
						column: x => x.AircraftId,
						principalTable: "Aircraft",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Flights_Airports_DepartureAirportId",
						column: x => x.DepartureAirportId,
						principalTable: "Airports",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Flights_Airports_DestinationAirportId",
						column: x => x.DestinationAirportId,
						principalTable: "Airports",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Aircraft_RegistrationNumber",
				table: "Aircraft",
				column: "RegistrationNumber",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Airports_Code",
				table: "Airports",
				column: "Code",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Customers_Email",
				table: "Customers",
				column: "Email",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Flights_AircraftId",
				table: "Flights",
				column: "AircraftId");

			migrationBuilder.CreateIndex(
				name: "IX_Flights_DepartureAirportId",
				table: "Flights",
				column: "DepartureAirportId");

			migrationBuilder.CreateIndex(
				name: "IX_Flights_DestinationAirportId",
				table: "Flights",
				column: "DestinationAirportId");

			migrationBuilder.CreateIndex(
				name: "IX_Flights_FlightNumber",
				table: "Flights",
				column: "FlightNumber");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Customers");

			migrationBuilder.DropTable(
				name: "Flights");

			migrationBuilder.DropTable(
				name: "Products");

			migrationBuilder.DropTable(
				name: "Aircraft");

			migrationBuilder.DropTable(
				name: "Airports");
		}
	}
}
