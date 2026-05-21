using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Features.Payments.DTOs;

namespace Rotinik.Features.Payments;

[Route("api/payments")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly PaymentSimulationService _paymentService;

    public PaymentController(PaymentSimulationService paymentService)
    {
        _paymentService = paymentService;
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
    {
        var userId = User.GetCurrentUserId();
        
        var transactionId = await _paymentService.GenerateCheckoutAsync(userId, dto.Amount);

        return Ok(new 
        { 
            transactionId = transactionId, 
            status = "Pending",
            message = "Pagamento criado. Para simular a aprovação, faça um POST em /api/payments/webhook-mock/{transactionId}" 
        });
    }

    [HttpPost("webhook-mock/{transactionId}")]
    public async Task<IActionResult> ApprovePaymentMock(string transactionId)
    {
        await _paymentService.ProcessPaymentSuccessAsync(transactionId);
        return Ok(new { message = "Webhook simulado com sucesso. Pagamento aprovado!" });
    }

    [Authorize]
    [HttpGet("status/{transactionId}")]
    public async Task<IActionResult> CheckStatus(string transactionId)
    {
        var payment = await _paymentService.GetByTransactionIdAsync(transactionId);
        if (payment == null) return NotFound();

        return Ok(new 
        { 
            transactionId = payment.TransactionId, 
            status = payment.Status.ToString() 
        });
    }
}