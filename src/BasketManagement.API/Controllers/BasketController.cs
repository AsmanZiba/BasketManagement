using Microsoft.AspNetCore.Mvc;
using BasketManagement.Application.Basket.Queries.GetOrCreateBasket;
using BasketManagement.Application.Basket.Commands.AddItemToBasket;
using BasketManagement.Application.Basket.Commands.UpdateBasketItem;
using BasketManagement.Application.Basket.Commands.RemoveBasketItem;
using BasketManagement.Application.Basket.Commands.ClearBasket;
using BasketManagement.API.Extensions;

namespace BasketManagement.API.Controllers;

[Route("api/v1/basket")]
public class BasketController : BaseController
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetOrCreate(long userId, CancellationToken cancellationToken)
    {
        var result = await QueryDispatcher.Send(new GetOrCreateBasketQuery(userId), cancellationToken);
        return result.ToApiResult();
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItem(long userId, [FromBody] AddBasketItemDTO item, CancellationToken cancellationToken)
    {
        var result = await CommandDispatcher.Send(new AddItemToBasketCommand(userId, item), cancellationToken);
        return result.ToApiResult();
    }

    [HttpPut("{userId}/items/{productId}")]
    public async Task<IActionResult> UpdateQuantity(long userId, long productId, [FromBody] int newQuantity, CancellationToken cancellationToken)
    {
        var result = await CommandDispatcher.Send(new UpdateBasketItemQuantityCommand(userId, productId, newQuantity), cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(long userId, long productId, CancellationToken cancellationToken)
    {
        var result = await CommandDispatcher.Send(new RemoveBasketItemCommand(userId, productId), cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> ClearBasket(long userId, CancellationToken cancellationToken)
    {
        var result = await CommandDispatcher.Send(new ClearBasketCommand(userId), cancellationToken);
        return result.ToApiResult();
    }
}