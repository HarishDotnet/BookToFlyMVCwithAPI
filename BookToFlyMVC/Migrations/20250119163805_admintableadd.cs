using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookToFlyMVC.Migrations
{
    /// <inheritdoc />
    public partial class admintableadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

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
        }
    }
}
