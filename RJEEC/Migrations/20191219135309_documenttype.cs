using Microsoft.EntityFrameworkCore.Migrations;

namespace RJEEC.Migrations
{
    public partial class documenttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Documents",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Documents",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
