using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookToFlyMVC.Migrations
{
    /// <inheritdoc />
    public partial class admintableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "UserRegistrationViewModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRegistrationViewModel",
                table: "UserRegistrationViewModel",
                column: "Username");

            migrationBuilder.CreateTable(
                name: "bookedTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookedTickets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bookedTickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRegistrationViewModel",
                table: "UserRegistrationViewModel");

            migrationBuilder.RenameTable(
                name: "UserRegistrationViewModel",
                newName: "User");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Username");
        }
    }
}
