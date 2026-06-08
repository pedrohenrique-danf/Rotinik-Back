using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEquippedToUserMedal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEquipped",
                table: "UserMedals",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEquipped",
                table: "UserMedals");
        }
    }
}
