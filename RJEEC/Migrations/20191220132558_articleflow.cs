using Microsoft.EntityFrameworkCore.Migrations;

namespace RJEEC.Migrations
{
    public partial class articleflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Authors_contactAuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_contactAuthorId",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "contactAuthorId",
                table: "Articles",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "contactAuthorId",
                table: "Articles",
                nullable: true,
                oldClrType: typeof(int));

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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
