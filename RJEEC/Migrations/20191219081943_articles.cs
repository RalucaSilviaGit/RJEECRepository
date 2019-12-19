using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RJEEC.Migrations
{
    public partial class articles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Magazine",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Volume = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    PublishingYear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magazine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    contactAuthorId = table.Column<int>(nullable: true),
                    KeyWords = table.Column<string>(nullable: true),
                    Authors = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    MagazineId = table.Column<int>(nullable: true),
                    AgreePublishingEthics = table.Column<bool>(nullable: false),
                    Comments = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Magazine_MagazineId",
                        column: x => x.MagazineId,
                        principalTable: "Magazine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_Authors_contactAuthorId",
                        column: x => x.contactAuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ArticleId",
                table: "Documents",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_MagazineId",
                table: "Articles",
                column: "MagazineId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_contactAuthorId",
                table: "Articles",
                column: "contactAuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Articles_ArticleId",
                table: "Documents",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Articles_ArticleId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Magazine");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ArticleId",
                table: "Documents");
        }
    }
}
