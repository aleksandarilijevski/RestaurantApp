using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class OnlineOrderAutoIncrementID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "TableArticleQuantitySequence");

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillCounter = table.Column<int>(type: "int", nullable: false),
                    CurrentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DataEntries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataEntryNumber = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataEntries", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<long>(type: "bigint", nullable: false),
                    FirstAndLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JMBG = table.Column<long>(type: "bigint", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ArticleDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleID = table.Column<int>(type: "int", nullable: false),
                    OriginalQuantity = table.Column<int>(type: "int", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "int", nullable: false),
                    DataEntryQuantity = table.Column<int>(type: "int", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataEntryID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ArticleDetails_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleDetails_DataEntries_DataEntryID",
                        column: x => x.DataEntryID,
                        principalTable: "DataEntries",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "OnlineOrders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<long>(type: "bigint", nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: true),
                    ApartmentNumber = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPayed = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineOrders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OnlineOrders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    InUse = table.Column<bool>(type: "bit", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    ArticleID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tables_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Tables_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ArticleDetailsTableArticleQuantity",
                columns: table => new
                {
                    ArticleDetailsID = table.Column<int>(type: "int", nullable: false),
                    TableArticleQuantitiesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetailsTableArticleQuantity", x => new { x.ArticleDetailsID, x.TableArticleQuantitiesID });
                    table.ForeignKey(
                        name: "FK_ArticleDetailsTableArticleQuantity_ArticleDetails_ArticleDetailsID",
                        column: x => x.ArticleDetailsID,
                        principalTable: "ArticleDetails",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableID = table.Column<int>(type: "int", nullable: true),
                    OnlineOrderID = table.Column<int>(type: "int", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cash = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Change = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bills_OnlineOrders_OnlineOrderID",
                        column: x => x.OnlineOrderID,
                        principalTable: "OnlineOrders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Bills_Tables_TableID",
                        column: x => x.TableID,
                        principalTable: "Tables",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Bills_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoldArticleDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticlePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoldQuantity = table.Column<int>(type: "int", nullable: false),
                    BillID = table.Column<int>(type: "int", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldArticleDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SoldArticleDetails_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoldTableArticleQuantity",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [TableArticleQuantitySequence]"),
                    TableID = table.Column<int>(type: "int", nullable: true),
                    OnlineOrderID = table.Column<int>(type: "int", nullable: true),
                    ArticleID = table.Column<int>(type: "int", nullable: false),
                    BillID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldTableArticleQuantity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SoldTableArticleQuantity_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SoldTableArticleQuantity_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SoldTableArticleQuantity_OnlineOrders_OnlineOrderID",
                        column: x => x.OnlineOrderID,
                        principalTable: "OnlineOrders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_SoldTableArticleQuantity_Tables_TableID",
                        column: x => x.TableID,
                        principalTable: "Tables",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TableArticleQuantity",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [TableArticleQuantitySequence]"),
                    TableID = table.Column<int>(type: "int", nullable: true),
                    OnlineOrderID = table.Column<int>(type: "int", nullable: true),
                    ArticleID = table.Column<int>(type: "int", nullable: false),
                    BillID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableArticleQuantity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableArticleQuantity_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableArticleQuantity_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TableArticleQuantity_OnlineOrders_OnlineOrderID",
                        column: x => x.OnlineOrderID,
                        principalTable: "OnlineOrders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TableArticleQuantity_Tables_TableID",
                        column: x => x.TableID,
                        principalTable: "Tables",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetails_ArticleID",
                table: "ArticleDetails",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetails_DataEntryID",
                table: "ArticleDetails",
                column: "DataEntryID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetailsTableArticleQuantity_TableArticleQuantitiesID",
                table: "ArticleDetailsTableArticleQuantity",
                column: "TableArticleQuantitiesID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_OnlineOrderID",
                table: "Bills",
                column: "OnlineOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_TableID",
                table: "Bills",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_UserID",
                table: "Bills",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OnlineOrders_UserID",
                table: "OnlineOrders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldArticleDetails_BillID",
                table: "SoldArticleDetails",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_ArticleID",
                table: "SoldTableArticleQuantity",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_BillID",
                table: "SoldTableArticleQuantity",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_OnlineOrderID",
                table: "SoldTableArticleQuantity",
                column: "OnlineOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_SoldTableArticleQuantity_TableID",
                table: "SoldTableArticleQuantity",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_ArticleID",
                table: "TableArticleQuantity",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_BillID",
                table: "TableArticleQuantity",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_OnlineOrderID",
                table: "TableArticleQuantity",
                column: "OnlineOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_TableArticleQuantity_TableID",
                table: "TableArticleQuantity",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_ArticleID",
                table: "Tables",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_UserID",
                table: "Tables",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleDetailsTableArticleQuantity");

            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "SoldArticleDetails");

            migrationBuilder.DropTable(
                name: "SoldTableArticleQuantity");

            migrationBuilder.DropTable(
                name: "TableArticleQuantity");

            migrationBuilder.DropTable(
                name: "ArticleDetails");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "DataEntries");

            migrationBuilder.DropTable(
                name: "OnlineOrders");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropSequence(
                name: "TableArticleQuantitySequence");
        }
    }
}
