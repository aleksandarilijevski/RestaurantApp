using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ReservedQuantityAddedArticleDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ArticleDetails",
                newName: "ReservedQuantity");

            migrationBuilder.AddColumn<int>(
                name: "OriginalQuantity",
                table: "ArticleDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalQuantity",
                table: "ArticleDetails");

            migrationBuilder.RenameColumn(
                name: "ReservedQuantity",
                table: "ArticleDetails",
                newName: "Quantity");
        }
    }
}
