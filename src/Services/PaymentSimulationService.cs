using Microsoft.EntityFrameworkCore;
using Rotinik.Data;
using Rotinik.Models;

namespace Rotinik.Services;

public class PaymentSimulationService
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;

    public PaymentSimulationService(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<string> GenerateCheckoutAsync(int userId, decimal amount)
    {
        var transactionId = $"tx_{Guid.NewGuid():N}";
        
        var payment = new Payment
        {
            TransactionId = transactionId,
            UserId = userId,
            Amount = amount,
            Status = PaymentStatus.Pending
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return transactionId;
    }

    public async Task ProcessPaymentSuccessAsync(string transactionId)
    {
        var payment = await _context.Payments.SingleOrDefaultAsync(p => p.TransactionId == transactionId);
        
        if (payment == null || payment.Status != PaymentStatus.Pending) return;

        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        await _userService.ActivatePremiumAsync(payment.UserId);
    }
}