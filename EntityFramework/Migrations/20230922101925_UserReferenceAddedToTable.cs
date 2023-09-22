using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UserReferenceAddedToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Tables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_UserID",
                table: "Tables",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Users_UserID",
                table: "Tables",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Users_UserID",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_UserID",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Tables");
        }
    }
}
