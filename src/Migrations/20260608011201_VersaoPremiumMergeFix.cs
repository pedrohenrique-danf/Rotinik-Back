using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class VersaoPremiumMergeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // O snapshot do EF foi corrigido nesta migration, mas o SQL gerado estava redundante/conflitante 
            // com a migration RefatoracaoEconomiaELoja. Portanto, deixamos o método Up vazio.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
