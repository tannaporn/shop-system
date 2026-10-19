using MediatR;
using Microsoft.AspNetCore.Mvc;
using shop_api_layer.Contracts;
using shop_api_layer.Contracts.Products;
using shop_api_layer.Features.Products;

namespace shop_api_layer.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetProducts(
        [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var products = await mediator.Send(new GetProductsQuery(search), cancellationToken);
        return Ok(new ApiResponse<List<ProductResponse>>(products));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> CreateProduct(
        [FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(null, new ApiResponse<ProductResponse>(product));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> UpdateProduct(
        Guid id, [FromBody] UpdateProductRequest body, CancellationToken cancellationToken)
    {
        var product = await mediator.Send(new UpdateProductCommand(id, body.Name, body.Description, body.Price), cancellationToken);
        return Ok(new ApiResponse<ProductResponse>(product));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/stock")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> UpdateStock(
        Guid id, [FromBody] UpdateStockRequest body, CancellationToken cancellationToken)
    {
        var product = await mediator.Send(new UpdateStockCommand(id, body.StockQuantity), cancellationToken);
        return Ok(new ApiResponse<ProductResponse>(product));
    }
}

public sealed record UpdateProductRequest(string Name, string? Description, decimal Price);
public sealed record UpdateStockRequest(int StockQuantity);
