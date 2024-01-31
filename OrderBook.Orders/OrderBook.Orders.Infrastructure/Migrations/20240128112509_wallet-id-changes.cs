using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderBook.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class walletidchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "Orders",
                newName: "OutGoingWalletId");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "OrderHistories",
                newName: "OutgoingWalletId");

            migrationBuilder.AddColumn<Guid>(
                name: "IncomingWalletId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IncomingWalletId",
                table: "OrderHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomingWalletId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IncomingWalletId",
                table: "OrderHistories");

            migrationBuilder.RenameColumn(
                name: "OutGoingWalletId",
                table: "Orders",
                newName: "WalletId");

            migrationBuilder.RenameColumn(
                name: "OutgoingWalletId",
                table: "OrderHistories",
                newName: "WalletId");
        }
    }
}
