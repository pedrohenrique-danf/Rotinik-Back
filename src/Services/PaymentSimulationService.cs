using Rotinik.Data.Repositories;
using Rotinik.Models;

namespace Rotinik.Services;

public interface IPaymentSimulationService
{
    Task<string> GenerateCheckoutAsync(int userId, decimal amount);
    Task ProcessPaymentSuccessAsync(string transactionId);
}

public class PaymentSimulationService : IPaymentSimulationService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserCommandService _userCommandService;

    public PaymentSimulationService(IPaymentRepository paymentRepository, IUserCommandService userCommandService)
    {
        _paymentRepository = paymentRepository;
        _userCommandService = userCommandService;
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

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return transactionId;
    }

    public async Task ProcessPaymentSuccessAsync(string transactionId)
    {
        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
        if (payment == null || payment.Status != PaymentStatus.Pending) return;

        payment.Status = PaymentStatus.Paid;
        payment.PaidAt = DateTime.UtcNow;
        
        await _paymentRepository.SaveChangesAsync();

        await _userCommandService.ActivatePremiumAsync(payment.UserId);
    }
}