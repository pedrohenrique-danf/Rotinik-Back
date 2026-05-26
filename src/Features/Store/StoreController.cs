using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotinik.Core.Extensions;
using Rotinik.Features.Store.DTOs; // <-- ADICIONE ISSO para corrigir o erro do DTO

namespace Rotinik.Features.Store;

[Authorize]
[Route("api/store")]
[ApiController]
public class StoreController : ControllerBase
{
    private const string StoreTag = "Store";
    private const string InventoryTag = "Inventory";

    private readonly StoreService _storeService;

    public StoreController(StoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet("items")]
    [Tags(StoreTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoreItems()
    {
        var items = await _storeService.GetAvailableItemsAsync();
        return Ok(new { data = items });
    }

    [HttpPost("items")] // Nova rota de criação que você adicionou
    [Tags(StoreTag)]
    public async Task<IActionResult> CreateItem(StoreItemCreateDto dto)
    {
        var result = await _storeService.CreateStoreItemAsync(dto);
        return Created("", result);
    }

    [HttpPost("buy/{itemId}")]
    [Tags(StoreTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BuyItem(int itemId)
    {
        var currentUserId = User.GetCurrentUserId();
        await _storeService.BuyItemAsync(currentUserId, itemId);
        
        return Ok(new { message = "Item purchased successfully!" });
    }

    [HttpGet("~/api/inventory/me")]
    [Tags(InventoryTag)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyInventory()
    {
        var currentUserId = User.GetCurrentUserId();
        var inventory = await _storeService.GetUserInventoryAsync(currentUserId);
        
        return Ok(new { data = inventory });
    }

    // CORRIGIDO: De [Patch(...)] para [HttpPatch(...)]
    [HttpPatch("~/api/inventory/{itemId}/equip")] 
    [Tags(InventoryTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EquipItem(int itemId)
    {
        var currentUserId = User.GetCurrentUserId();
        await _storeService.EquipItemAsync(currentUserId, itemId);
        
        return NoContent();
    }
}