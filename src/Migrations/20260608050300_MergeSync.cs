using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class MergeSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Empty migration because these columns were already added in dev branch migrations.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
