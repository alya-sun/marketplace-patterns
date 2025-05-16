using Marketplace.Adapters;
using Marketplace.Models;
using Marketplace.Services;
using Marketplace.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Logger>();
builder.Services.AddSingleton<Cart>();

builder.Services.AddTransient<ProductIterator>(provider =>
{
    var catalog = provider.GetRequiredService<List<Product>>();
    return new ProductIterator(catalog);
});

builder.Services.AddTransient<IDeliveryService, ExternalCourierAdapter>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<OrderHandler>(_ =>
{
    var validate = new ValidateOrderHandler();
    var discount = new ApplyDiscountHandler();
    var log = new LogOrderHandler();

    validate.SetNext(discount);
    discount.SetNext(log);

    return validate;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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