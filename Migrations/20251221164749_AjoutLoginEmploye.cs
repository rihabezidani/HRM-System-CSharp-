using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rh.Migrations
{
    
    public partial class AjoutLoginEmploye : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_responsablesrh_ResponsableRHId",
                table: "employees");

            migrationBuilder.AlterColumn<int>(
                name: "ResponsableRHId",
                table: "employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "employees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "employees",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_responsablesrh_ResponsableRHId",
                table: "employees",
                column: "ResponsableRHId",
                principalTable: "responsablesrh",
                principalColumn: "Id");
        }

       
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_responsablesrh_ResponsableRHId",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "employees");

            migrationBuilder.AlterColumn<int>(
                name: "ResponsableRHId",
                table: "employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_responsablesrh_ResponsableRHId",
                table: "employees",
                column: "ResponsableRHId",
                principalTable: "responsablesrh",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
