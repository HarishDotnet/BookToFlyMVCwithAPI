using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightDetailApi.Migrations
{
    /// <inheritdoc />
    public partial class migrationsupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    BookingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlightId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfTravel = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.BookingId);
                });

            migrationBuilder.CreateTable(
                name: "DomesticFlightDetails",
                columns: table => new
                {
                    FlightId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AirlineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArrivalTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DepartureTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvailableDays = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomesticFlightDetails", x => x.FlightId);
                });

            migrationBuilder.CreateTable(
                name: "InternationalFlightDetails",
                columns: table => new
                {
                    FlightId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AirlineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArrivalTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DepartureTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvailableDays = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternationalFlightDetails", x => x.FlightId);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "DomesticFlightDetails");

            migrationBuilder.DropTable(
                name: "InternationalFlightDetails");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
