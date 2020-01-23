using Microsoft.EntityFrameworkCore.Migrations;

namespace RJEEC.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Articles_contactAuthorId",
                table: "Articles",
                column: "contactAuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Authors_contactAuthorId",
                table: "Articles",
                column: "contactAuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Authors_contactAuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_contactAuthorId",
                table: "Articles");
        }
    }
}
