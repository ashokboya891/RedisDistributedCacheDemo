using Microsoft.Extensions.DependencyInjection;
using RedisDistributedCacheDemo.IServices;
using RedisDistributedCacheDemo.Services;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")
);

// Add Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Update if Redis runs on a different server/port
    options.InstanceName = "RedisTesting";
});
// Add custom Redis service for reusable operations
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddHostedService<SubscriberService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
