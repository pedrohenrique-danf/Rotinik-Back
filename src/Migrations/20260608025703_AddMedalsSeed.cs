using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddMedalsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Medals",
                columns: new[] { "Id", "Description", "IconUrl", "Name", "TargetValue", "TriggerType" },
                values: new object[,]
                {
                    { 1, "Conclua sua 1ª tarefa.", "🎯", "Primeiro Passo", 1, 2 },
                    { 2, "Conclua 10 tarefas.", "⚡", "Produtivo", 10, 2 },
                    { 3, "Conclua 50 tarefas.", "🔥", "Máquina de Tarefas", 50, 2 },
                    { 4, "Conclua sua primeira rotina.", "📅", "Rotineiro", 1, 4 },
                    { 5, "Conclua 3 rotinas.", "💪", "Firme e Forte", 3, 4 },
                    { 6, "Conclua 10 rotinas.", "👑", "Mestre da Rotina", 10, 4 },
                    { 7, "Junte 100 Moedas.", "💰", "Acumulador", 100, 1 },
                    { 8, "Junte 500 Moedas.", "💎", "Rico", 500, 1 },
                    { 9, "Adquira a versão Premium.", "⭐", "Apoiador Premium", 1, 5 },
                    { 10, "Compre seu primeiro cosmético.", "🛍️", "Consumidor", 1, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Medals",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
