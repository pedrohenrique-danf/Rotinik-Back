using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedDataEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Tasks"" SET ""Priority"" = 'Low' WHERE ""Priority"" = '1';
                UPDATE ""Tasks"" SET ""Priority"" = 'Moderate' WHERE ""Priority"" = '2';
                UPDATE ""Tasks"" SET ""Priority"" = 'Important' WHERE ""Priority"" = '3';
                UPDATE ""Tasks"" SET ""Priority"" = 'Urgent' WHERE ""Priority"" = '4';

                UPDATE ""Tasks"" SET ""Frequency"" = 'Daily' WHERE ""Frequency"" = 'daily';
                UPDATE ""Tasks"" SET ""Frequency"" = 'Monthly' WHERE ""Frequency"" = 'monthly';
                UPDATE ""Tasks"" SET ""Frequency"" = 'Yearly' WHERE ""Frequency"" = 'yearly';
                UPDATE ""Tasks"" SET ""Frequency"" = 'Daily' WHERE ""Frequency"" = 'weekly'; -- TaskFrequency has no weekly, map to Daily or None? Actually rotinik has Daily/Monthly/Yearly. Let's map weekly to Daily for tasks, wait. TaskFrequency enum only has Daily, Monthly, Yearly. Let's just use Daily.

                UPDATE ""Routines"" SET ""Frequency"" = 'Daily' WHERE ""Frequency"" = 'daily';
                UPDATE ""Routines"" SET ""Frequency"" = 'Weekly' WHERE ""Frequency"" = 'weekly';
                UPDATE ""Routines"" SET ""Frequency"" = 'Monthly' WHERE ""Frequency"" = 'monthly';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Tasks"" SET ""Priority"" = '1' WHERE ""Priority"" = 'Low';
                UPDATE ""Tasks"" SET ""Priority"" = '2' WHERE ""Priority"" = 'Moderate';
                UPDATE ""Tasks"" SET ""Priority"" = '3' WHERE ""Priority"" = 'Important';
                UPDATE ""Tasks"" SET ""Priority"" = '4' WHERE ""Priority"" = 'Urgent';

                UPDATE ""Tasks"" SET ""Frequency"" = 'daily' WHERE ""Frequency"" = 'Daily';
                UPDATE ""Routines"" SET ""Frequency"" = 'daily' WHERE ""Frequency"" = 'Daily';
                UPDATE ""Routines"" SET ""Frequency"" = 'weekly' WHERE ""Frequency"" = 'Weekly';
            ");
        }
    }
}
