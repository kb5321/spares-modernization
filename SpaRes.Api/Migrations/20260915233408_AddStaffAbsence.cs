using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaRes.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffAbsence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffAbsence",
                columns: table => new
                {
                    StaffAbsenceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StaffId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAbsence", x => x.StaffAbsenceId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffAbsence");
        }
    }
}
