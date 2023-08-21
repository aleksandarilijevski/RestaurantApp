using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ArticleDetailsToList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoldTableArticleQuantity_ArticleDetails_ArticleDetailsID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropForeignKey(
                name: "FK_TableArticleQuantity_ArticleDetails_ArticleDetailsID",
                table: "TableArticleQuantity");

            migrationBuilder.DropIndex(
                name: "IX_TableArticleQuantity_ArticleDetailsID",
                table: "TableArticleQuantity");

            migrationBuilder.DropIndex(
                name: "IX_SoldTableArticleQuantity_ArticleDetailsID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "ArticleDetailsID",
                table: "TableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "ArticleDetailsID",
                table: "SoldTableArticleQuantity");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArticleDetails_TableArticleQuantityID",
                table: "ArticleDetails");

            migrationBuilder.DropColumn(
                name: "TableArticleQuantityID",
                table: "ArticleDetails");

            migrationBuilder.AddColumn<int>(
                name: "ArticleDetailsID",
                table: "TableArticleQuantity",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArticleDetailsID",
                table: "SoldTableArticleQuantity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_ArticleDetailsID",
                table: "TableArticleQuantity",
                column: "ArticleDetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_ArticleDetailsID",
                table: "SoldTableArticleQuantity",
                column: "ArticleDetailsID");

            migrationBuilder.AddForeignKey(
                name: "FK_SoldTableArticleQuantity_ArticleDetails_ArticleDetailsID",
                table: "SoldTableArticleQuantity",
                column: "ArticleDetailsID",
                principalTable: "ArticleDetails",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantity_ArticleDetails_ArticleDetailsID",
                table: "TableArticleQuantity",
                column: "ArticleDetailsID",
                principalTable: "ArticleDetails",
                principalColumn: "ID");
        }
    }
}
