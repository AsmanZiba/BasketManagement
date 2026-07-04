using BasketManagement.API.Middleware;
using BasketManagement.Application.Behaviors;
using BasketManagement.Infrastructure.Persistence;
using BasketManagement.Infrastructure.Persistence.Repositories;
using BasketManagement.Infrastructure.Services.MessageBus;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using BasketManagement.Application.Basket.Commands.ExpireBaskets;
using BasketManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// افزودن سرویس‌ها
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// MediatR با Behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// DI
builder.Services.RegisterDependencies(
    typeof(Program).Assembly,                      // Api
    typeof(ExpireBasketsCommandHandler).Assembly,  // Application
    typeof(BasketRepository).Assembly              // Infrastructure
);

// Configuration
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Background Services (RabbitMQ Consumer و Timer)
builder.Services.AddHostedService<RabbitMQConsumerService>();
builder.Services.AddHostedService<RabbitMQTimerPublisher>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// اعمال Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();