using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rh.Migrations
{
 
    public partial class AddMotifToConge : Migration
    {
       
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Commentaire",
                table: "conges",
                newName: "Motif");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDemande",
                table: "conges",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

       
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDemande",
                table: "conges");

            migrationBuilder.RenameColumn(
                name: "Motif",
                table: "conges",
                newName: "Commentaire");
        }
    }
}
