using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;

namespace Rotinik.Features.Payments;

[Route("api/payments")]
[ApiController]
[Tags("Payments (Simulação)")]
public class PaymentController : ControllerBase
{
    private readonly PaymentSimulationService _paymentService;

    public PaymentController(PaymentSimulationService paymentService)
    {
        _paymentService = paymentService;
    }

    [Authorize]
    [HttpPost("checkout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApprovePaymentMock(string transactionId)
    {
        await _paymentService.ProcessPaymentSuccessAsync(transactionId);
        return Ok(new { message = "Webhook simulado com sucesso. Pagamento aprovado!" });
    }

    [Authorize]
    [HttpGet("status/{transactionId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckStatus(string transactionId)
    {
        var payment = await _paymentService.GetByTransactionIdAsync(transactionId);
        
        if (payment == null) 
            return NotFound(new { message = "Transaction not found." });

        return Ok(new 
        { 
            transactionId = payment.TransactionId, 
            status = payment.Status.ToString() 
        });
    }
}