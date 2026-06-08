using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddMedalRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RewardCoins",
                table: "Medals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RewardPoints",
                table: "Medals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 20, 50 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardCoins",
                table: "Medals");

            migrationBuilder.DropColumn(
                name: "RewardPoints",
                table: "Medals");
        }
    }
}
