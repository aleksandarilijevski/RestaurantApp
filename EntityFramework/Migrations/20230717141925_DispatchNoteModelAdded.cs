using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class DispatchNoteModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DispatchNoteID",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DispatchNotes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchNoteNumber = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchNotes", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_DispatchNoteID",
                table: "Articles",
                column: "DispatchNoteID");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_DispatchNotes_DispatchNoteID",
                table: "Articles",
                column: "DispatchNoteID",
                principalTable: "DispatchNotes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_DispatchNotes_DispatchNoteID",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "DispatchNotes");

            migrationBuilder.DropIndex(
                name: "IX_Articles_DispatchNoteID",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "DispatchNoteID",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "Places",
                table: "Tables",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
