using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class OrderOnlineModelCreatedEFContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OnlineOrderID",
                table: "TableArticleQuantity",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OnlineOrderID",
                table: "SoldTableArticleQuantity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OnlineOrders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<long>(type: "bigint", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    ApartmentNumber = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineOrders", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_OnlineOrderID",
                table: "TableArticleQuantity",
                column: "OnlineOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_OnlineOrderID",
                table: "SoldTableArticleQuantity",
                column: "OnlineOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_SoldTableArticleQuantity_OnlineOrders_OnlineOrderID",
                table: "SoldTableArticleQuantity",
                column: "OnlineOrderID",
                principalTable: "OnlineOrders",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TableArticleQuantity_OnlineOrders_OnlineOrderID",
                table: "TableArticleQuantity",
                column: "OnlineOrderID",
                principalTable: "OnlineOrders",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoldTableArticleQuantity_OnlineOrders_OnlineOrderID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropForeignKey(
                name: "FK_TableArticleQuantity_OnlineOrders_OnlineOrderID",
                table: "TableArticleQuantity");

            migrationBuilder.DropTable(
                name: "OnlineOrders");

            migrationBuilder.DropIndex(
                name: "IX_TableArticleQuantity_OnlineOrderID",
                table: "TableArticleQuantity");

            migrationBuilder.DropIndex(
                name: "IX_SoldTableArticleQuantity_OnlineOrderID",
                table: "SoldTableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "OnlineOrderID",
                table: "TableArticleQuantity");

            migrationBuilder.DropColumn(
                name: "OnlineOrderID",
                table: "SoldTableArticleQuantity");
        }
    }
}
