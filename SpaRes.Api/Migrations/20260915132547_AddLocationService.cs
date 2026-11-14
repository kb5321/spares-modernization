using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaRes.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationServices",
                columns: table => new
                {
                    LocationServiceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Available = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationServices", x => x.LocationServiceId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationServices");
        }
    }
}
