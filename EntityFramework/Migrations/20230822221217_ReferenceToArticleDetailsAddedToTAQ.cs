using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceToArticleDetailsAddedToTAQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleDetailsID",
                table: "TableArticleQuantity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArticleDetailsID",
                table: "SoldTableArticleQuantity",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantity_ArticleDetails_ArticleDetailsID",
                table: "TableArticleQuantity",
                column: "ArticleDetailsID",
                principalTable: "ArticleDetails",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
