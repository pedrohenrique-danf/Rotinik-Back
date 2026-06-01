using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimatedMinutesToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "Tasks",
                type: "integer",
                nullable: false,
                defaultValue: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "Tasks");
        }
    }
}
