using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaRes.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservationId = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedByStaffId = table.Column<int>(type: "INTEGER", nullable: false),
                    LogTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LogInfo = table.Column<string>(type: "TEXT", nullable: false),
                    LogFieldNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    LogValueBefore = table.Column<string>(type: "TEXT", nullable: false),
                    LogValueAfter = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResLogs", x => x.LogId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResLogs");
        }
    }
}
