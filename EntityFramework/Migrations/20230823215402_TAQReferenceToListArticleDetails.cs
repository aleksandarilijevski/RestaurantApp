using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TAQReferenceToListArticleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleDetailsID",
                table: "TableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "ArticleDetailsID",
                table: "SoldTableArticleQuantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
