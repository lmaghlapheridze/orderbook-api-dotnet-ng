using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderBook.Wallets.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyType = table.Column<int>(type: "integer", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_Wallet_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "WalletOperations",
                columns: table => new
                {
                    WalletOperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletOperations", x => x.WalletOperationId);
                    table.ForeignKey(
                        name: "FK_WalletOperations_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "WalletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_UserId",
                table: "Wallet",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletOperations_WalletId",
                table: "WalletOperations",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletOperations");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
