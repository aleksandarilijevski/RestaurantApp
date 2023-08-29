using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyTAQArticleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArticleDetails_TableArticleQuantityID",
                table: "ArticleDetails");

            migrationBuilder.DropColumn(
                name: "TableArticleQuantityID",
                table: "ArticleDetails");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleDetailsTableArticleQuantity");

            migrationBuilder.AddColumn<int>(
                name: "TableArticleQuantityID",
                table: "ArticleDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetails_TableArticleQuantityID",
                table: "ArticleDetails",
                column: "TableArticleQuantityID");
        }
    }
}
