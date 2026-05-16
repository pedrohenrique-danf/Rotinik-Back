using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik_Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaSuaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPremium",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPremium",
                table: "Users");
        }
    }
}
