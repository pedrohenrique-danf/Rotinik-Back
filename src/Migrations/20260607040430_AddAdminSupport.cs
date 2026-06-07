using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculatedAt",
                table: "DailyUserSummaries");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "user");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Routines",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CoinsEarned",
                table: "DailyUserSummaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ShopItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Rarity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Discount = table.Column<double>(type: "double precision", nullable: true),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 1, "cosmetic", "Um gato ninja como mascote", null, "🐱", true, "Gato Ninja", 150, "rare" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { 2, "cosmetic", "Um dragao mistico roxo", null, "🐉", "Dragao Roxo", 250, "epic" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 3, "cosmetic", "Um unicornio com brilho especial", null, "🦄", true, "Unicornio Brilhoso", 200, "epic" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[,]
                {
                    { 4, "boost", "Ganhe o dobro de XP pelas proximas 7 dias", null, "⚡", "Dobro de XP (7 dias)", 500, "rare" },
                    { 5, "boost", "Proteja seu streak por 1 falha", null, "🛡️", "Protetor de Streak", 300, "epic" }
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 6, "theme", "Tema com cores neon brilhantes", null, "💎", true, "Tema Neon", 200, "rare" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[,]
                {
                    { 7, "theme", "Tema com cores verdes naturais", null, "🌿", "Tema Floresta", 150, "common" },
                    { 8, "badge", "Mostra que voce e rapido", null, "🏃", "Placa: Speedrunner", 100, "common" },
                    { 9, "badge", "A placa do verdadeiro lendario", null, "👑", "Placa: Lenda", 1000, "legendary" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopItems");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CoinsEarned",
                table: "DailyUserSummaries");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Routines",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CalculatedAt",
                table: "DailyUserSummaries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
