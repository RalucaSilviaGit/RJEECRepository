using Microsoft.EntityFrameworkCore.Migrations;

namespace RJEEC.Migrations
{
    public partial class Magazines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPath",
                table: "Magazines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackCoverPath",
                table: "Magazines",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Magazines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Articles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPath",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "BackCoverPath",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Articles");
        }
    }
}
