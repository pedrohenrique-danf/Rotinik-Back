using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedalRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 1,
                column: "RewardCoins",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 50, 100 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 200, 500 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 25, 100 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 100, 300 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 500, 1000 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 0, 150 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 0, 500 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "RewardCoins", "RewardPoints" },
                values: new object[] { 1000, 2000 });

            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 10,
                column: "RewardCoins",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 1,
                column: "RewardCoins",
                value: 20);

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
                column: "RewardCoins",
                value: 20);
        }
    }
}
