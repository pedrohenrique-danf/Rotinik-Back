using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class AddSpaceShopItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEquipped",
                table: "UserShopItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "ShopItems",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "avatar", "O inicio de uma grande jornada espacial.", "👨‍🚀", "Astronauta Novato", 100, "common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "avatar", "Um visitante de outra galaxia.", "👽", "Extraterrestre", 200, "rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "avatar", "Tecnologia avancada de marte.", "🤖", "Robo Marciano", 300, "rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Icon", "Name", "Rarity" },
                values: new object[] { "avatar", "Lider da frota espacial.", "🧑‍✈️", "Comandante Estelar", "epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "avatar", "O puro poder do universo.", "🌌", "Entidade Cosmica", 1000, "legendary" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Description", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { "border", "Poeira espacial rodando seu perfil.", "2px dashed #9ca3af", false, "Borda de Asteroides", 150, "common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "Description", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { "border", "Um aro de luz de orbita baixa.", "3px solid #38bdf8", true, "Borda Neon Orbita", 250, "rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "border", "A propulsao do foguete no seu perfil.", "3px solid #f97316", "Borda Chama de Foguete", 350, "epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "Description", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { "border", "Uma aura cosmica misteriosa.", "4px double #8b5cf6", true, "Borda Materia Escura", 600, "epic" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[,]
                {
                    { 10, "border", "O brilho intenso de uma estrela explodindo.", null, "4px solid #fbbf24", "Borda Supernova", 1200, "legendary" },
                    { 11, "level_icon", "Orbita basica de comunicacao.", null, "📡", "Satelite", 100, "common" },
                    { 12, "level_icon", "Exploracao do nosso vizinho.", null, "🌕", "Lua", 200, "common" }
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 13, "level_icon", "Voando alem das nuvens.", null, "🚀", true, "Foguete Espacial", 400, "rare" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { 14, "level_icon", "Uma nave nao identificada.", null, "🛸", "Disco Voador", 700, "epic" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 15, "level_icon", "Um rastro iluminado e magico.", null, "🌠", true, "Estrela Cadente", 1500, "legendary" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[,]
                {
                    { 16, "background", "Cores cinzas do solo lunar.", null, "linear-gradient(135deg, #1f2937, #374151)", "Fundo Superficie Lunar", 200, "common" },
                    { 17, "background", "Uma vista para as estrelas noturnas.", null, "linear-gradient(135deg, #0f172a, #1e1b4b)", "Fundo Ceu Estrelado", 400, "rare" }
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 18, "background", "Uma mistura quente de gases estelares.", null, "linear-gradient(135deg, #7c2d12, #9a3412)", true, "Fundo Nebulosa Solar", 600, "epic" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { 19, "background", "Sugando toda a luz do universo.", null, "radial-gradient(circle, #000000 0%, #171717 100%)", "Fundo Buraco Negro", 900, "epic" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 20, "background", "Cores cosmicas brilhantes.", null, "linear-gradient(135deg, #1e1b4b 0%, #4c1d95 50%, #0ea5e9 100%)", true, "Fundo Aurora Boreal Estelar", 2000, "legendary" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { 21, "navbar", "Metal basico de naves.", null, "#1e293b", "Nav Bar Padrao Espacial", 150, "common" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 22, "navbar", "Brilho roxo espacial profundo.", null, "#4c1d95", true, "Nav Bar Ametista Galactica", 300, "rare" });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "Name", "Price", "Rarity" },
                values: new object[,]
                {
                    { 23, "navbar", "Vermelho e quente como o planeta.", null, "#9f1239", "Nav Bar Marte", 500, "epic" },
                    { 24, "navbar", "Um tom cosmico azul escuro.", null, "#0f172a", "Nav Bar Via Lactea", 800, "epic" }
                });

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Category", "Description", "Discount", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { 25, "navbar", "O brilho maximo da tecnologia.", null, "linear-gradient(90deg, #6d28d9, #2563eb, #db2777)", true, "Nav Bar Velocidade da Luz", 2500, "legendary" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DropColumn(
                name: "IsEquipped",
                table: "UserShopItems");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "ShopItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "cosmetic", "Um gato ninja como mascote", "🐱", "Gato Ninja", 150, "rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "cosmetic", "Um dragao mistico roxo", "🐉", "Dragao Roxo", 250, "epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "cosmetic", "Um unicornio com brilho especial", "🦄", "Unicornio Brilhoso", 200, "epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Icon", "Name", "Rarity" },
                values: new object[] { "boost", "Ganhe o dobro de XP pelas proximas 7 dias", "⚡", "Dobro de XP (7 dias)", "rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "boost", "Proteja seu streak por 1 falha", "🛡️", "Protetor de Streak", 300, "epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Category", "Description", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { "theme", "Tema com cores neon brilhantes", "💎", true, "Tema Neon", 200, "rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "Description", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { "theme", "Tema com cores verdes naturais", "🌿", false, "Tema Floresta", 150, "common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Description", "Icon", "Name", "Price", "Rarity" },
                values: new object[] { "badge", "Mostra que voce e rapido", "🏃", "Placa: Speedrunner", 100, "common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "Description", "Icon", "IsNew", "Name", "Price", "Rarity" },
                values: new object[] { "badge", "A placa do verdadeiro lendario", "👑", false, "Placa: Lenda", 1000, "legendary" });
        }
    }
}
