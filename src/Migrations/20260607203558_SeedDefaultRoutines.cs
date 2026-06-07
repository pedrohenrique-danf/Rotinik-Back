using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rotinik.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRoutines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""Routines"" (""Title"", ""Description"", ""Category"", ""Frequency"", ""IsDefault"", ""CreatedAt"")
                VALUES 
                ('Ritual de Energia Matinal', 'Comece o dia com disposição e saúde física.', 'Saude', 'daily', true, CURRENT_TIMESTAMP),
                ('Foco Total', 'Bloco de estudos diário para aprender algo novo.', 'Estudos', 'daily', true, CURRENT_TIMESTAMP),
                ('Produtividade Profissional', 'Organize e execute seu trabalho com excelência.', 'Trabalho', 'daily', true, CURRENT_TIMESTAMP),
                ('Lar Doce Lar', 'Pequenas tarefas para manter a casa limpa e organizada.', 'Casa', 'daily', true, CURRENT_TIMESTAMP),
                ('Conexões Reais', 'Mantenha o contato com quem importa para você.', 'Social', 'weekly', true, CURRENT_TIMESTAMP),
                ('Paz Interior', 'Momentos de reflexão para acalmar a mente.', 'Mindfulness', 'daily', true, CURRENT_TIMESTAMP),
                ('Descompressão', 'Tempo dedicado a você mesmo e seus hobbies.', 'Lazer', 'weekly', true, CURRENT_TIMESTAMP),
                ('Hábito de Leitura', 'Leia um pouco todos os dias para expandir a mente.', 'Leitura', 'daily', true, CURRENT_TIMESTAMP);
            ");

            migrationBuilder.Sql(@"
                DO $$
                DECLARE 
                    r1 INT; r2 INT; r3 INT; r4 INT; r5 INT; r6 INT; r7 INT; r8 INT;
                BEGIN
                    SELECT ""Id"" INTO r1 FROM ""Routines"" WHERE ""Title"" = 'Ritual de Energia Matinal' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r2 FROM ""Routines"" WHERE ""Title"" = 'Foco Total' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r3 FROM ""Routines"" WHERE ""Title"" = 'Produtividade Profissional' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r4 FROM ""Routines"" WHERE ""Title"" = 'Lar Doce Lar' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r5 FROM ""Routines"" WHERE ""Title"" = 'Conexões Reais' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r6 FROM ""Routines"" WHERE ""Title"" = 'Paz Interior' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r7 FROM ""Routines"" WHERE ""Title"" = 'Descompressão' AND ""IsDefault"" = true LIMIT 1;
                    SELECT ""Id"" INTO r8 FROM ""Routines"" WHERE ""Title"" = 'Hábito de Leitura' AND ""IsDefault"" = true LIMIT 1;

                    INSERT INTO ""Tasks"" (""RoutineId"", ""Title"", ""Description"", ""Frequency"", ""Priority"", ""IsCompleted"", ""XpReward"", ""CoinReward"", ""CreatedAt"", ""Order"") VALUES
                    (r1, 'Beber 500ml de água', 'Hidratação ao acordar', 'daily', 2, false, 10, 5, CURRENT_TIMESTAMP, 0),
                    (r1, 'Alongamento de 5 minutos', 'Ativação corporal', 'daily', 2, false, 15, 5, CURRENT_TIMESTAMP, 1),
                    (r1, 'Tomar café da manhã saudável', '', 'daily', 2, false, 20, 10, CURRENT_TIMESTAMP, 2),

                    (r2, 'Revisar material anterior', 'Revisão ativa', 'daily', 2, false, 15, 5, CURRENT_TIMESTAMP, 0),
                    (r2, 'Estudar conteúdo novo por 30m', 'Foco profundo', 'daily', 3, false, 30, 15, CURRENT_TIMESTAMP, 1),

                    (r3, 'Planejar o dia', 'Listar top 3 tarefas', 'daily', 3, false, 20, 10, CURRENT_TIMESTAMP, 0),
                    (r3, 'Limpar caixa de email', 'Inbox zero', 'daily', 1, false, 10, 5, CURRENT_TIMESTAMP, 1),
                    (r3, 'Fazer networking', 'Falar com 1 contato', 'weekly', 2, false, 25, 10, CURRENT_TIMESTAMP, 2),

                    (r4, 'Arrumar a cama', 'Primeira vitória do dia', 'daily', 1, false, 10, 5, CURRENT_TIMESTAMP, 0),
                    (r4, 'Lavar a louça', '', 'daily', 2, false, 15, 5, CURRENT_TIMESTAMP, 1),
                    (r4, 'Organizar a mesa', '', 'daily', 2, false, 10, 5, CURRENT_TIMESTAMP, 2),

                    (r5, 'Mandar mensagem para um amigo', '', 'weekly', 2, false, 15, 10, CURRENT_TIMESTAMP, 0),
                    (r5, 'Ligar para a família', '', 'weekly', 3, false, 25, 15, CURRENT_TIMESTAMP, 1),

                    (r6, 'Meditar por 10 minutos', 'Respirar fundo', 'daily', 3, false, 20, 10, CURRENT_TIMESTAMP, 0),
                    (r6, 'Escrever no diário', 'Gratidão do dia', 'daily', 2, false, 15, 5, CURRENT_TIMESTAMP, 1),

                    (r7, 'Assistir um episódio de série', '', 'weekly', 1, false, 10, 5, CURRENT_TIMESTAMP, 0),
                    (r7, 'Jogar algo ou praticar hobby', 'Pelo menos 1 hora', 'weekly', 2, false, 20, 10, CURRENT_TIMESTAMP, 1),

                    (r8, 'Ler 10 páginas', 'Qualquer livro', 'daily', 3, false, 20, 10, CURRENT_TIMESTAMP, 0),
                    (r8, 'Fazer anotações da leitura', '', 'weekly', 2, false, 15, 5, CURRENT_TIMESTAMP, 1);
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""Tasks"" WHERE ""RoutineId"" IN (SELECT ""Id"" FROM ""Routines"" WHERE ""IsDefault"" = true);
                DELETE FROM ""Routines"" WHERE ""IsDefault"" = true;
            ");
        }
    }
}
