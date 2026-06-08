using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class VersaoPremiumMergeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserShopItem_ShopItems_ShopItemId",
                table: "UserShopItem");

            migrationBuilder.DropForeignKey(
                name: "FK_UserShopItem_Users_UserId",
                table: "UserShopItem");

            migrationBuilder.RenameTable(
                name: "UserShopItem",
                newName: "UserShopItems");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WalletTransactions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserShopItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ShopItemId",
                table: "UserShopItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserShopItems",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchasedAt",
                table: "UserShopItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserShopItems",
                table: "UserShopItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserShopItems_ShopItemId",
                table: "UserShopItems",
                column: "ShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShopItems_UserId",
                table: "UserShopItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserShopItems_ShopItems_ShopItemId",
                table: "UserShopItems",
                column: "ShopItemId",
                principalTable: "ShopItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserShopItems_Users_UserId",
                table: "UserShopItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserShopItems_ShopItems_ShopItemId",
                table: "UserShopItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserShopItems_Users_UserId",
                table: "UserShopItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserShopItems",
                table: "UserShopItems");

            migrationBuilder.DropIndex(
                name: "IX_UserShopItems_ShopItemId",
                table: "UserShopItems");

            migrationBuilder.DropIndex(
                name: "IX_UserShopItems_UserId",
                table: "UserShopItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserShopItems");

            migrationBuilder.DropColumn(
                name: "PurchasedAt",
                table: "UserShopItems");

            migrationBuilder.RenameTable(
                name: "UserShopItems",
                newName: "UserShopItem");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WalletTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserShopItem",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ShopItemId",
                table: "UserShopItem",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_UserShopItem_ShopItems_ShopItemId",
                table: "UserShopItem",
                column: "ShopItemId",
                principalTable: "ShopItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserShopItem_Users_UserId",
                table: "UserShopItem",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
