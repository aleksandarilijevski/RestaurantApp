using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TableArticleQuantityModelCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "TableArticleQuantities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableID = table.Column<int>(type: "int", nullable: false),
                    ArticleID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableArticleQuantities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableArticleQuantities_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableArticleQuantities_Tables_TableID",
                        column: x => x.TableID,
                        principalTable: "Tables",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantities_ArticleID",
                table: "TableArticleQuantities",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantities_TableID",
                table: "TableArticleQuantities",
                column: "TableID");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleTable_Tables_TablesID",
                table: "ArticleTable",
                column: "TablesID",
                principalTable: "Tables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleTable_Tables_TablesID",
                table: "ArticleTable");

            migrationBuilder.DropTable(
                name: "TableArticleQuantities");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Tables");

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
    }
}
