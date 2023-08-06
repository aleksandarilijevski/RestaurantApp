using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TableARticleQuantityRemvoedFromBillModel_AndReplacedWithTableModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TableArticleQuantities_Bills_BillID",
                table: "TableArticleQuantities");

            migrationBuilder.DropIndex(
                name: "IX_TableArticleQuantities_BillID",
                table: "TableArticleQuantities");

            migrationBuilder.DropColumn(
                name: "BillID",
                table: "TableArticleQuantities");

            migrationBuilder.AddColumn<int>(
                name: "TableID",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_TableID",
                table: "Bills",
                column: "TableID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Tables_TableID",
                table: "Bills",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Tables_TableID",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_TableID",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "TableID",
                table: "Bills");

            migrationBuilder.AddColumn<int>(
                name: "BillID",
                table: "TableArticleQuantities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantities_BillID",
                table: "TableArticleQuantities",
                column: "BillID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantities_Bills_BillID",
                table: "TableArticleQuantities",
                column: "BillID",
                principalTable: "Bills",
                principalColumn: "ID");
        }
    }
}
