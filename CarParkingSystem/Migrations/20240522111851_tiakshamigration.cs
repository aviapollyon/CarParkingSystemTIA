using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarParkingSystem.Migrations
{
    public partial class tiakshamigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaultReported",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultReported", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Noticeboard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Noticeboard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fault",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaultReportedId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fault", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fault_FaultReported_FaultReportedId",
                        column: x => x.FaultReportedId,
                        principalTable: "FaultReported",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaultId = table.Column<int>(type: "int", nullable: false),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianReport_Fault_FaultId",
                        column: x => x.FaultId,
                        principalTable: "Fault",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fault_FaultReportedId",
                table: "Fault",
                column: "FaultReportedId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianReport_FaultId",
                table: "TechnicianReport",
                column: "FaultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Noticeboard");

            migrationBuilder.DropTable(
                name: "TechnicianReport");

            migrationBuilder.DropTable(
                name: "Fault");

            migrationBuilder.DropTable(
                name: "FaultReported");
        }
    }
}
