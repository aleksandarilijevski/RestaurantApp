using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddedArticlesToTableModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
