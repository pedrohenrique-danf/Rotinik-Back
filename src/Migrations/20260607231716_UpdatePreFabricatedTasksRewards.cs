using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePreFabricatedTasksRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Tasks"" SET ""XpReward"" = 10, ""CoinReward"" = 5 WHERE ""Priority"" = 'Low';
                UPDATE ""Tasks"" SET ""XpReward"" = 20, ""CoinReward"" = 10 WHERE ""Priority"" = 'Moderate';
                UPDATE ""Tasks"" SET ""XpReward"" = 30, ""CoinReward"" = 15 WHERE ""Priority"" = 'Important';
                UPDATE ""Tasks"" SET ""XpReward"" = 50, ""CoinReward"" = 25 WHERE ""Priority"" = 'Urgent';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // O downgrade não restaura valores antigos específicos, pois a regra nova passou a ser o padrão.
            // Poderíamos restaurar para 10 e 5, mas deixaremos como está.
        }
    }
}
