using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TableDoesntInheritsBaseEntityAnymore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTable");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Tables");

            migrationBuilder.AddColumn<int>(
                name: "ArticleID",
                table: "Tables",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_ArticleID",
                table: "Tables",
                column: "ArticleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Articles_ArticleID",
                table: "Tables",
                column: "ArticleID",
                principalTable: "Articles",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Articles_ArticleID",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_ArticleID",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ArticleID",
                table: "Tables");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Tables",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Tables",
                type: "datetime2",
                nullable: true);

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
    }
}
