using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Rotinik.Core.Data;

// O NOME DO ENDEREÇO DEVE SER EXATAMENTE ESTE:
namespace Rotinik.Features.Users.Workers;

public class AccountDeletionWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AccountDeletionWorker> _logger;

    public AccountDeletionWorker(IServiceProvider serviceProvider, ILogger<AccountDeletionWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Executando varredura de exclusão de contas...");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Busca usuários cuja data agendada já passou
                var usersToDelete = await context.Users
                    .Where(u => u.DeletionScheduledFor.HasValue && u.DeletionScheduledFor.Value <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                if (usersToDelete.Any())
                {
                    context.Users.RemoveRange(usersToDelete);
                    await context.SaveChangesAsync(stoppingToken);
                    
                    _logger.LogInformation($"{usersToDelete.Count} contas foram permanentemente excluídas.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar executar a exclusão definitiva de contas.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}