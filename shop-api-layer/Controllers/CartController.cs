using MediatR;
using Microsoft.AspNetCore.Mvc;
using shop_api_layer.Contracts;
using shop_api_layer.Contracts.Cart;
using shop_api_layer.Features.Cart;

namespace shop_api_layer.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController(IMediator mediator, IConfiguration config) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(config["DevUser:UserId"] ?? "00000000-0000-0000-0000-000000000001");

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartResponse>>> GetCart(CancellationToken cancellationToken)
    {
        var cart = await mediator.Send(new GetCartQuery(GetUserId()), cancellationToken);
        return Ok(new ApiResponse<CartResponse>(cart));
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CartResponse>>> AddItem(
        [FromBody] AddCartItemRequest body, CancellationToken cancellationToken)
    {
        var cart = await mediator.Send(new AddCartItemCommand(GetUserId(), body.ProductId, body.Quantity), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<CartResponse>(cart));
    }

    [HttpPut("items/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CartResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateItem(
        Guid id, [FromBody] UpdateCartItemRequest body, CancellationToken cancellationToken)
    {
        var cart = await mediator.Send(new UpdateCartItemCommand(id, body.Quantity, GetUserId()), cancellationToken);
        return Ok(new ApiResponse<CartResponse>(cart));
    }

    [HttpDelete("items/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveItem(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveCartItemCommand(id, GetUserId()), cancellationToken);
        return NoContent();
    }

    [HttpDelete("items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        await mediator.Send(new ClearCartCommand(GetUserId()), cancellationToken);
        return NoContent();
    }
}

public sealed record AddCartItemRequest(Guid ProductId, int Quantity);
public sealed record UpdateCartItemRequest(int Quantity);
