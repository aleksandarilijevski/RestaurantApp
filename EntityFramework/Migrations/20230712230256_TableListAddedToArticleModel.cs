using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TableListAddedToArticleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Tables_TableID",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_TableID",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "TableID",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "ArticleTable",
                columns: table => new
                {
                    ArticlesID = table.Column<int>(type: "int", nullable: false),
                    TablesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTable", x => new { x.ArticlesID, x.TablesID });
                    table.ForeignKey(
                        name: "FK_ArticleTable_Articles_ArticlesID",
                        column: x => x.ArticlesID,
                        principalTable: "Articles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTable_Tables_TablesID",
                        column: x => x.TablesID,
                        principalTable: "Tables",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTable_TablesID",
                table: "ArticleTable",
                column: "TablesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTable");

            migrationBuilder.AddColumn<int>(
                name: "TableID",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TableID",
                table: "Articles",
                column: "TableID");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Tables_TableID",
                table: "Articles",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID");
        }
    }
}
