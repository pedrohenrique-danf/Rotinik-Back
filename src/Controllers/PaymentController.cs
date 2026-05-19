using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Data.Repositories;
using Rotinik.Services;
using System.Security.Claims;

namespace Rotinik.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentSimulationService _paymentService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IServiceScopeFactory _scopeFactory;

    public PaymentController(
        IPaymentSimulationService paymentService, 
        IPaymentRepository paymentRepository,
        IServiceScopeFactory scopeFactory)
    {
        _paymentService = paymentService;
        _paymentRepository = paymentRepository;
        _scopeFactory = scopeFactory;
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();
        
        var transactionId = await _paymentService.GenerateCheckoutAsync(userId, 29.90m);

        _ = Task.Run(async () =>
        {
            await Task.Delay(10000); 

            using var scope = _scopeFactory.CreateScope();
            var bgPaymentService = scope.ServiceProvider.GetRequiredService<IPaymentSimulationService>();
            
            await bgPaymentService.ProcessPaymentSuccessAsync(transactionId);
        });

        return Ok(new 
        { 
            transactionId = transactionId, 
            status = "Pending",
            message = "Pagamento aguardando confirmação. (O gateway virtual aprovará em 10 segundos)." 
        });
    }

    [Authorize]
    [HttpGet("status/{transactionId}")]
    public async Task<IActionResult> CheckStatus(string transactionId)
    {
        var payment = await _paymentRepository.GetByTransactionIdAsync(transactionId);
        if (payment == null) return NotFound();

        return Ok(new 
        { 
            transactionId = payment.TransactionId, 
            status = payment.Status.ToString() 
        });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userIdClaim, out int userId);
        return userId;
    }
}