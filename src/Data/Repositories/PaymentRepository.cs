using Microsoft.EntityFrameworkCore;
using Rotinik.Models;

namespace Rotinik.Data.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId) =>
        await _context.Payments.SingleOrDefaultAsync(p => p.TransactionId == transactionId);

    public async Task AddAsync(Payment payment) =>
        await _context.Payments.AddAsync(payment);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}