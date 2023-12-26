using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thss0.Web.Migrations
{
    public partial class ResultName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Substance",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Results",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Substance");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Results");
        }
    }
}
