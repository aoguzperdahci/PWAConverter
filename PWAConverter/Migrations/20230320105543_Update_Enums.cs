using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWAConverter.Migrations
{
    public partial class Update_Enums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "Sources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisplayMode",
                table: "Manifests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Orientation",
                table: "Manifests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                table: "Sources");

            migrationBuilder.DropColumn(
                name: "DisplayMode",
                table: "Manifests");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "Manifests");
        }
    }
}
