using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Exceptions;
using Rotinik.Core.Data;
using Rotinik.Features.Users;

namespace Rotinik.Features.Payments;

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
        
        if (payment == null) 
            throw new NotFoundException("Transaction not found.");
            
        if (payment.Status != PaymentStatus.Pending) 
            throw new ConflictException("Transaction has already been processed.");

        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        await _userService.ActivatePremiumAsync(payment.UserId);
    }
    
    public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
    {
        return await _context.Payments
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.TransactionId == transactionId);
    }
}