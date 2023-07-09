using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TableModelUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Bills_BillID",
                table: "Tables");

            migrationBuilder.AlterColumn<int>(
                name: "BillID",
                table: "Tables",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Bills_BillID",
                table: "Tables",
                column: "BillID",
                principalTable: "Bills",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Bills_BillID",
                table: "Tables");

            migrationBuilder.AlterColumn<int>(
                name: "BillID",
                table: "Tables",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Bills_BillID",
                table: "Tables",
                column: "BillID",
                principalTable: "Bills",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
