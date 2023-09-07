using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TAQEdited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoldTableArticleQuantity_Tables_TableID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropForeignKey(
                name: "FK_TableArticleQuantity_Tables_TableID",
                table: "TableArticleQuantity");

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "TableArticleQuantity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "SoldTableArticleQuantity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SoldTableArticleQuantity_Tables_TableID",
                table: "SoldTableArticleQuantity",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantity_Tables_TableID",
                table: "TableArticleQuantity",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoldTableArticleQuantity_Tables_TableID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropForeignKey(
                name: "FK_TableArticleQuantity_Tables_TableID",
                table: "TableArticleQuantity");

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "TableArticleQuantity",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "SoldTableArticleQuantity",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SoldTableArticleQuantity_Tables_TableID",
                table: "SoldTableArticleQuantity",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantity_Tables_TableID",
                table: "TableArticleQuantity",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
