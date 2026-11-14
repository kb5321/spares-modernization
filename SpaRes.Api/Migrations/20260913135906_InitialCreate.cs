using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaRes.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Time = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookedById = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    Request = table.Column<bool>(type: "INTEGER", nullable: false),
                    PartOfPackage = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRepeatClient = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
