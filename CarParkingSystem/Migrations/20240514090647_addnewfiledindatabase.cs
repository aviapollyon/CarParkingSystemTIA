using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarParkingSystem.Migrations
{
    public partial class addnewfiledindatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPaid",
                table: "StudentViolations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPaid",
                table: "StudentViolations");
        }
    }
}
