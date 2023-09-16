using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class DataEntryArticlesReferenceRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_DataEntries_DataEntryID",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_DataEntryID",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "DataEntryID",
                table: "Articles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataEntryID",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DataEntryID",
                table: "Articles",
                column: "DataEntryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_DataEntries_DataEntryID",
                table: "Articles",
                column: "DataEntryID",
                principalTable: "DataEntries",
                principalColumn: "ID");
        }
    }
}
