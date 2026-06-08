using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Core.Exceptions;

// 1. Atualizado para o namespace correto da Carteira
namespace Rotinik.Features.Wallet;

// 2. Rota atualizada para refletir o domínio de economia
[Route("api/wallet")]
[ApiController]
[Authorize] // Toda a carteira exige autenticação
public class WalletController : ControllerBase
{
    private readonly WalletService _walletService;

    public WalletController(WalletService walletService)
    {
        // 3. Construtor corrigido para injeção de dependência adequada
        _walletService = walletService;
    }

    // 4. Substitui o antigo "CheckStatus"
    // Retorna o extrato completo de ganhos e gastos do usuário
    [HttpGet("history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTransactionHistory()
    {
        var userId = User.GetCurrentUserId();
        
        var transactions = await _walletService.GetUserTransactionsAsync(userId);

        return Ok(transactions);
    }

    // 5. Substitui o antigo "Checkout / Webhook"
    // Endpoint para o usuário comprar o Premium usando suas moedas do jogo
    [HttpPost("premium")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PurchasePremium()
    {
        var userId = User.GetCurrentUserId();
        
        // Defina o custo do Premium em moedas (pode vir do banco ou de uma configuração no futuro)
        int premiumCost = 1000; 

        try
        {
            await _walletService.PurchasePremiumAsync(userId, premiumCost);
            
            return Ok(new 
            { 
                message = "Premium ativado com sucesso usando suas moedas!",
                coinsSpent = premiumCost
            });
        }
        catch (ConflictException ex)
        {
            // Trata a falta de saldo ou usuário que já é premium
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}