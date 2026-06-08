using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Features.Shop.DTOs;
using System.Security.Claims;

namespace Rotinik.Features.Shop;

[Authorize]
[Route("api/shop")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly ShopService _shopService;

    public ShopController(ShopService shopService)
    {
        _shopService = shopService;
    }

    [HttpGet("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListItems()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int userId))
            return Unauthorized();

        var result = await _shopService.ListAllItemsAsync(userId);
        return Ok(result);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("admin/items")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateItem(ShopItemCreateDto dto)
    {
        var result = await _shopService.CreateItemAsync(dto);
        return StatusCode(201, result);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("admin/items/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(int id, ShopItemUpdateDto dto)
    {
        await _shopService.UpdateItemAsync(id, dto);
        return NoContent();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("admin/items/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        await _shopService.DeleteItemAsync(id);
        return NoContent();
    }
    
    [HttpPost("items/{id}/purchase")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PurchaseItem(int id)
    {
        // Extrai o ID como string do Token
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Converte para int com segurança
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return Unauthorized();

        try
        {
            // Agora passamos int e int
            await _shopService.PurchaseItemAsync(id, userId);
            return Ok(new { message = "Compra realizada com sucesso!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPost("items/{id}/equip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EquipItem(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            return Unauthorized();

        try
        {
            await _shopService.EquipItemAsync(id, userId);
            return Ok(new { message = "Item equipado/desequipado com sucesso!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
