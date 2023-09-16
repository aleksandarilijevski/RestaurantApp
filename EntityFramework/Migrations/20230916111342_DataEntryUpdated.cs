using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class DataEntryUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataEntryID",
                table: "ArticleDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetails_DataEntryID",
                table: "ArticleDetails",
                column: "DataEntryID");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleDetails_DataEntries_DataEntryID",
                table: "ArticleDetails",
                column: "DataEntryID",
                principalTable: "DataEntries",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleDetails_DataEntries_DataEntryID",
                table: "ArticleDetails");

            migrationBuilder.DropIndex(
                name: "IX_ArticleDetails_DataEntryID",
                table: "ArticleDetails");

            migrationBuilder.DropColumn(
                name: "DataEntryID",
                table: "ArticleDetails");
        }
    }
}
