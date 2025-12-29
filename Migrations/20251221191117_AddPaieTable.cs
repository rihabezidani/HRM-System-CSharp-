using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rh.Migrations
{
    
    public partial class AddPaieTable : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bulletinspaie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeId = table.Column<int>(type: "int", nullable: false),
                    Periode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalaireBrut = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Retenues = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalaireNet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bulletinspaie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bulletinspaie_employees_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_bulletinspaie_EmployeId",
                table: "bulletinspaie",
                column: "EmployeId");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bulletinspaie");
        }
    }
}
