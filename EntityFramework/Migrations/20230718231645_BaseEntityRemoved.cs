using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_BaseEntities_ArticleID",
                table: "BaseEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_BaseEntities_BillID",
                table: "BaseEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_BaseEntities_DispatchNoteID",
                table: "BaseEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseEntities_BaseEntities_Table_BillID",
                table: "BaseEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseEntities",
                table: "BaseEntities");

            migrationBuilder.DropIndex(
                name: "IX_BaseEntities_ArticleID",
                table: "BaseEntities");

            migrationBuilder.DropIndex(
                name: "IX_BaseEntities_BillID",
                table: "BaseEntities");

            migrationBuilder.DropIndex(
                name: "IX_BaseEntities_DispatchNoteID",
                table: "BaseEntities");

            migrationBuilder.DropIndex(
                name: "IX_BaseEntities_Table_BillID",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "ArticleID",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Article_Quantity",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "BillID",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "DispatchNoteID",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "DispatchNoteNumber",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "EntryPrice",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Table_BillID",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "BaseEntities");

            migrationBuilder.DropColumn(
                name: "Waiter_Barcode",
                table: "BaseEntities");

            migrationBuilder.RenameTable(
                name: "BaseEntities",
                newName: "Waiters");

            migrationBuilder.AlterColumn<long>(
                name: "JMBG",
                table: "Waiters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Waiters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Barcode",
                table: "Waiters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Waiters",
                table: "Waiters",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DispatchNotes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchNoteNumber = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchNotes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    BillID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tables_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillID = table.Column<int>(type: "int", nullable: true),
                    DispatchNoteID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Articles_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Articles_DispatchNotes_DispatchNoteID",
                        column: x => x.DispatchNoteID,
                        principalTable: "DispatchNotes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ArticleDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ArticleDetails_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetails_ArticleID",
                table: "ArticleDetails",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_BillID",
                table: "Articles",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DispatchNoteID",
                table: "Articles",
                column: "DispatchNoteID");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_BillID",
                table: "Tables",
                column: "BillID");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "ArticleDetails");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "DispatchNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Waiters",
                table: "Waiters");

            migrationBuilder.RenameTable(
                name: "Waiters",
                newName: "BaseEntities");

            migrationBuilder.AlterColumn<long>(
                name: "JMBG",
                table: "BaseEntities",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "BaseEntities",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "Barcode",
                table: "BaseEntities",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "ArticleID",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Article_Quantity",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "BaseEntities",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillID",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaseEntities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DispatchNoteID",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DispatchNoteNumber",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EntryPrice",
                table: "BaseEntities",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BaseEntities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "BaseEntities",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Table_BillID",
                table: "BaseEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "BaseEntities",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "BaseEntities",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Waiter_Barcode",
                table: "BaseEntities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseEntities",
                table: "BaseEntities",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntities_ArticleID",
                table: "BaseEntities",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntities_BillID",
                table: "BaseEntities",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntities_DispatchNoteID",
                table: "BaseEntities",
                column: "DispatchNoteID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntities_Table_BillID",
                table: "BaseEntities",
                column: "Table_BillID");



            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_BaseEntities_ArticleID",
                table: "BaseEntities",
                column: "ArticleID",
                principalTable: "BaseEntities",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_BaseEntities_BillID",
                table: "BaseEntities",
                column: "BillID",
                principalTable: "BaseEntities",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_BaseEntities_DispatchNoteID",
                table: "BaseEntities",
                column: "DispatchNoteID",
                principalTable: "BaseEntities",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseEntities_BaseEntities_Table_BillID",
                table: "BaseEntities",
                column: "Table_BillID",
                principalTable: "BaseEntities",
                principalColumn: "ID");
        }
    }
}
