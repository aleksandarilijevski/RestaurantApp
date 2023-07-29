using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class tableId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTable_Tables_TablesID",
                table: "ArticleTable");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Tables",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TablesID",
                table: "ArticleTable",
                newName: "TablesId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleTable_TablesID",
                table: "ArticleTable",
                newName: "IX_ArticleTable_TablesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTable_Tables_TablesId",
                table: "ArticleTable",
                column: "TablesId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTable_Tables_TablesId",
                table: "ArticleTable");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tables",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "TablesId",
                table: "ArticleTable",
                newName: "TablesID");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleTable_TablesId",
                table: "ArticleTable",
                newName: "IX_ArticleTable_TablesID");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTable_Tables_TablesID",
                table: "ArticleTable",
                column: "TablesID",
                principalTable: "Tables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
