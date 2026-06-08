using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndCosmeticEquip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEquipped",
                table: "UserShopItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: "avatar");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: "avatar");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: "avatar");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: "navbar");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "Category",
                value: "border");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "Category",
                value: "background");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "Category",
                value: "background");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "Category",
                value: "level_icon");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "Category",
                value: "level_icon");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsBanned",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEquipped",
                table: "UserShopItems");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: "cosmetic");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: "cosmetic");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: "cosmetic");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                column: "Category",
                value: "boost");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                column: "Category",
                value: "boost");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 6,
                column: "Category",
                value: "theme");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 7,
                column: "Category",
                value: "theme");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 8,
                column: "Category",
                value: "badge");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 9,
                column: "Category",
                value: "badge");
        }
    }
}
