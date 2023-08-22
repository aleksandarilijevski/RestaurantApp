using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyIsRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleDetailsTableArticleQuantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleDetailsTableArticleQuantity",
                columns: table => new
                {
                    ArticleDetailsID = table.Column<int>(type: "int", nullable: false),
                    TableArticleQuantitiesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetailsTableArticleQuantity", x => new { x.ArticleDetailsID, x.TableArticleQuantitiesID });
                    table.ForeignKey(
                        name: "FK_ArticleDetailsTableArticleQuantity_ArticleDetails_ArticleDetailsID",
                        column: x => x.ArticleDetailsID,
                        principalTable: "ArticleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetailsTableArticleQuantity_TableArticleQuantitiesID",
                table: "ArticleDetailsTableArticleQuantity",
                column: "TableArticleQuantitiesID");
        }
    }
}
