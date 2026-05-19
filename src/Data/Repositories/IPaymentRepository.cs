using Rotinik.Models;

namespace Rotinik.Data.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByTransactionIdAsync(string transactionId);
    Task AddAsync(Payment payment);
    Task SaveChangesAsync();
}