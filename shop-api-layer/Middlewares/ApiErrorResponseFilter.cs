using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace shop_api_layer.Middlewares;

// Always run so automatic API-controller validation errors use the same envelope.
public sealed class ApiErrorResponseFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult { Value: ProblemDetails problem } result)
        {
            context.Result = new ObjectResult(ApiProblemDetails.ToResponse(problem, context.HttpContext))
            {
                StatusCode = result.StatusCode ?? problem.Status ?? 500,
                ContentTypes = { "application/json" }
            };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context) { }
}
