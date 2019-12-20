using Microsoft.EntityFrameworkCore.Migrations;

namespace RJEEC.Migrations
{
    public partial class magazines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Magazine_MagazineId",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Magazine",
                table: "Magazine");

            migrationBuilder.RenameTable(
                name: "Magazine",
                newName: "Magazines");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Magazines",
                table: "Magazines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Magazines_MagazineId",
                table: "Articles",
                column: "MagazineId",
                principalTable: "Magazines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Magazines_MagazineId",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Magazines",
                table: "Magazines");

            migrationBuilder.RenameTable(
                name: "Magazines",
                newName: "Magazine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Magazine",
                table: "Magazine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Magazine_MagazineId",
                table: "Articles",
                column: "MagazineId",
                principalTable: "Magazine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
