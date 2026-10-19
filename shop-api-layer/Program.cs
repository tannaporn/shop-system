using shop_api_layer.Data;
using shop_api_layer.Middlewares;
using Microsoft.EntityFrameworkCore;
using shop_api_layer.Authentication;
using Microsoft.OpenApi.Models;
using shop_api_layer.Enums;

var builder = WebApplication.CreateBuilder(args);

var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(
    builder.Configuration.GetConnectionString("DefaultConnection"));
dataSourceBuilder.MapEnum<OrderStatus>("order_status", nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource)
           .UseSnakeCaseNamingConvention());

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()));

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add<ApiErrorResponseFilter>());
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = ApiProblemDetails.Customize);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT access token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }] = Array.Empty<string>()
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusContext =>
{
    var context = statusContext.HttpContext;
    var factory = context.RequestServices.GetRequiredService<Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory>();
    var problem = factory.CreateProblemDetails(context, context.Response.StatusCode);
    await context.Response.WriteAsJsonAsync(ApiProblemDetails.ToResponse(problem, context));
});
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
