using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TableArticleQuantityBillReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillID",
                table: "TableArticleQuantity",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillID",
                table: "SoldTableArticleQuantity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_BillID",
                table: "TableArticleQuantity",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_BillID",
                table: "SoldTableArticleQuantity",
                column: "BillID");

            migrationBuilder.AddForeignKey(
                name: "FK_SoldTableArticleQuantity_Bills_BillID",
                table: "SoldTableArticleQuantity",
                column: "BillID",
                principalTable: "Bills",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantity_Bills_BillID",
                table: "TableArticleQuantity",
                column: "BillID",
                principalTable: "Bills",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoldTableArticleQuantity_Bills_BillID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropForeignKey(
                name: "FK_TableArticleQuantity_Bills_BillID",
                table: "TableArticleQuantity");

            migrationBuilder.DropIndex(
                name: "IX_TableArticleQuantity_BillID",
                table: "TableArticleQuantity");

            migrationBuilder.DropIndex(
                name: "IX_SoldTableArticleQuantity_BillID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "BillID",
                table: "TableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "BillID",
                table: "SoldTableArticleQuantity");
        }
    }
}
