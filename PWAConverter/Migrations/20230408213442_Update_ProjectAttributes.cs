using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWAConverter.Migrations
{
    public partial class Update_ProjectAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Manifests_ManifestId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ManifestId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ManifestId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectDetail",
                table: "Projects",
                newName: "ProjectDetailId");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "Projects",
                newName: "IconId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Manifests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Manifests_ProjectId",
                table: "Manifests",
                column: "ProjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Manifests_Projects_ProjectId",
                table: "Manifests",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manifests_Projects_ProjectId",
                table: "Manifests");

            migrationBuilder.DropIndex(
                name: "IX_Manifests_ProjectId",
                table: "Manifests");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Manifests");

            migrationBuilder.RenameColumn(
                name: "ProjectDetailId",
                table: "Projects",
                newName: "ProjectDetail");

            migrationBuilder.RenameColumn(
                name: "IconId",
                table: "Projects",
                newName: "Icon");

            migrationBuilder.AddColumn<Guid>(
                name: "ManifestId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ManifestId",
                table: "Projects",
                column: "ManifestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Manifests_ManifestId",
                table: "Projects",
                column: "ManifestId",
                principalTable: "Manifests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
