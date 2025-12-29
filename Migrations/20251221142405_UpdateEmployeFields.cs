using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rh.Migrations
{
    
    public partial class UpdateEmployeFields : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateEmbauche",
                table: "employees",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "employees",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateArchivage",
                table: "employees",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNaissance",
                table: "employees",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Departement",
                table: "employees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EstActif",
                table: "employees",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JoursCongesRestants",
                table: "employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Matricule",
                table: "employees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MotifArchivage",
                table: "employees",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "employees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "DateArchivage",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "DateNaissance",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Departement",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "EstActif",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "JoursCongesRestants",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Matricule",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "MotifArchivage",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "employees");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateEmbauche",
                table: "employees",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
