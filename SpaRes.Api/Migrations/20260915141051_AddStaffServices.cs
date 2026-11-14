using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaRes.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffServices",
                columns: table => new
                {
                    StaffServiceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StaffId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffServices", x => x.StaffServiceId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffServices");
        }
    }
}
