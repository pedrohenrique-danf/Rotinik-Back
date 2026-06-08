using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Users;

namespace Rotinik.Features.Wallet;

public class WalletService
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;

    public WalletService(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    // 1. ADICIONAR MOEDAS (Substitui o antigo GenerateCheckout)
    // Usado quando o usuário completa tarefas, rotinas, etc.
    public async Task AddCoinsAsync(int userId, int amount, TransactionSource source, string description)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("Usuário não encontrado.");

        // Adiciona o saldo
        user.Coins += amount;

        // Registra o extrato
        var transaction = new WalletTransaction
        {
            UserId = userId,
            Amount = amount,
            Currency = CurrencyType.Coins,
            Type = TransactionType.Earned,
            Source = source,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    // 2. ATIVAR PREMIUM COM MOEDAS (Substitui o antigo ProcessPaymentSuccess)
    // O usuário paga com as moedas que ganhou jogando para virar Premium
    public async Task PurchasePremiumAsync(int userId, int premiumCostInCoins)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) 
            throw new NotFoundException("Usuário não encontrado.");
            
        // Blindagem contra saldo negativo
        if (user.Coins < premiumCostInCoins) 
            throw new ConflictException("Moedas insuficientes para assinar o Premium.");

        // Deduz o saldo
        user.Coins -= premiumCostInCoins;

        // Registra o gasto no extrato
        var transaction = new WalletTransaction
        {
            UserId = userId,
            Amount = -premiumCostInCoins, // Gasto é negativo no extrato
            Currency = CurrencyType.Coins,
            Type = TransactionType.Spent,
            Source = TransactionSource.Store,
            Description = "Assinatura Premium",
            CreatedAt = DateTime.UtcNow
        };

        _context.WalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Aciona o serviço de usuário que você já injetou para liberar a flag
        await _userService.ActivatePremiumAsync(userId);
    }
    
    // 3. EXTRATO DA CARTEIRA (Substitui a busca por TransactionId)
    // Usado para mostrar o histórico de ganhos e gastos na tela do jogador
    public async Task<List<WalletTransaction>> GetUserTransactionsAsync(int userId)
    {
        return await _context.WalletTransactions
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}