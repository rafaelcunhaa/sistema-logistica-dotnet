using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Messaging;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

// Endpoint para criar um novo pedido
app.MapPost("/pedidos", (PedidoCreateDto dto) => {

    // Criamos um pedido a partir dos dados recebidos
    var pedido = new Pedido
    {
        Cliente = dto.Cliente,
        Produto = dto.Produto,
        Quantidade = dto.Quantidade,
        ValorTotal = dto.Valor,
        CriadoEm = DateTime.UtcNow,
    };
    // Publicamos a mensagem no RabbitMQ    
    var publisher = new RabbitMqPublisher();
    publisher.Publicar(QueueNames.PedidosCriados, pedido);
    // Retornamos status 201 (Created) com o pedido criado
    return Results.Created($"/pedidos/{pedido.Id}", pedido);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
