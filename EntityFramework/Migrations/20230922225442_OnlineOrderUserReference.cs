using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class OnlineOrderUserReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "OnlineOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OnlineOrders_UserID",
                table: "OnlineOrders",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineOrders_Users_UserID",
                table: "OnlineOrders",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineOrders_Users_UserID",
                table: "OnlineOrders");

            migrationBuilder.DropIndex(
                name: "IX_OnlineOrders_UserID",
                table: "OnlineOrders");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "OnlineOrders");
        }
    }
}
