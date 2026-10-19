using System.Net;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace shop_api_layer.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}",
                    System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier);
                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                await WriteErrorResponse(context, ex);
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                shop_api_layer.Contracts.ForbiddenException => (int)HttpStatusCode.Forbidden,
                shop_api_layer.Exceptions.ConflictException => (int)HttpStatusCode.Conflict,
                shop_api_layer.Exceptions.UnprocessableEntityException => 422,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problem = factory.CreateProblemDetails(
                context,
                context.Response.StatusCode,
                detail: context.Response.StatusCode < 500 ? ex.Message : null);
            await context.Response.WriteAsJsonAsync(ApiProblemDetails.ToResponse(problem, context));
        }
    }
}
