using MediatR;
using Microsoft.AspNetCore.Mvc;
using shop_api_layer.Contracts;
using shop_api_layer.Contracts.Orders;
using shop_api_layer.Enums;
using shop_api_layer.Features.Orders;

namespace shop_api_layer.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IMediator mediator, IConfiguration config) : ControllerBase
{
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> Checkout(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var order = await mediator.Send(new CheckoutCommand(userId), cancellationToken);
        return CreatedAtAction(nameof(Checkout), new ApiResponse<OrderResponse>(order));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<OrderResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OrderResponse>>>> GetOrders(
        [FromQuery] OrderStatus? status, CancellationToken cancellationToken)
    {
        var orders = await mediator.Send(new GetOrdersQuery(status), cancellationToken);
        return Ok(new ApiResponse<List<OrderResponse>>(orders));
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOrderStatus(
        Guid id, [FromBody] UpdateOrderStatusRequest body, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateOrderStatusCommand(id, body.Status), cancellationToken);
        return NoContent();
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<List<OrderResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OrderResponse>>>> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var orders = await mediator.Send(new GetMyOrdersQuery(userId), cancellationToken);
        return Ok(new ApiResponse<List<OrderResponse>>(orders));
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await mediator.Send(new CancelOrderCommand(id, userId), cancellationToken);
        return NoContent();
    }

    private Guid GetUserId() => Guid.Parse(config["DevUser:UserId"] ?? "00000000-0000-0000-0000-000000000001");
}

public sealed record UpdateOrderStatusRequest(OrderStatus Status);
