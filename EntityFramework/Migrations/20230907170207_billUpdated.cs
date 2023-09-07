using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class billUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Tables_TableID",
                table: "Bills");

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "Bills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OnlineOrderID",
                table: "Bills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_OnlineOrderID",
                table: "Bills",
                column: "OnlineOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_OnlineOrders_OnlineOrderID",
                table: "Bills",
                column: "OnlineOrderID",
                principalTable: "OnlineOrders",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Tables_TableID",
                table: "Bills",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_OnlineOrders_OnlineOrderID",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Tables_TableID",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_OnlineOrderID",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "OnlineOrderID",
                table: "Bills");

            migrationBuilder.AlterColumn<int>(
                name: "TableID",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Tables_TableID",
                table: "Bills",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
