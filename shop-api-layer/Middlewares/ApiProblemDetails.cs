using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using shop_api_layer.Contracts;

namespace shop_api_layer.Middlewares;

public static class ApiProblemDetails
{
    public static ApiResponse<object?> ToResponse(ProblemDetails problem, HttpContext context)
    {
        Customize(new ProblemDetailsContext { HttpContext = context, ProblemDetails = problem });
        return ApiResponse<object?>.Failure(new ApiError(
            problem.Extensions["code"]!.ToString()!,
            problem.Detail ?? problem.Title ?? "The request failed.",
            problem.Extensions["traceId"]!.ToString()!,
            (problem as ValidationProblemDetails)?.Errors));
    }

    public static void Customize(ProblemDetailsContext context)
    {
        var problem = context.ProblemDetails;
        problem.Instance ??= context.HttpContext.Request.Path;
        problem.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        if (!problem.Extensions.ContainsKey("code"))
        {
            problem.Extensions["code"] = problem is ValidationProblemDetails
                ? "VALIDATION_ERROR"
                : problem.Status switch
                {
                    400 => "BAD_REQUEST",
                    401 => "UNAUTHORIZED",
                    403 => "FORBIDDEN",
                    404 => "NOT_FOUND",
                    405 => "METHOD_NOT_ALLOWED",
                    409 => "CONFLICT",
                    415 => "UNSUPPORTED_MEDIA_TYPE",
                    429 => "TOO_MANY_REQUESTS",
                    >= 500 => "INTERNAL_SERVER_ERROR",
                    _ => "HTTP_ERROR"
                };
        }

        if (problem.Status >= 500)
        {
            problem.Title = "An internal server error occurred.";
            problem.Detail = null;
        }
    }
}
