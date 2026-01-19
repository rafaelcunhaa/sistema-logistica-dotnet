using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Messaging;
using Microsoft.EntityFrameworkCore;
using Logistica.Pedidos.Api.Data;
using System.Linq;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.MapPost("/pedidos", async (PedidoCreateDto dto, AppDbContext db) => {

    // Criamos um pedido a partir dos dados recebidos
    var pedido = new Pedido 
    {
        Cliente = dto.Cliente,
        Produto = dto.Produto,
        Quantidade = dto.Quantidade,
        ValorTotal = dto.Valor,
        CriadoEm = DateTime.UtcNow,
    };

    var erros = new List<string>();

    if (string.IsNullOrWhiteSpace(dto.Cliente))
        erros.Add("Cliente é obrigatório. ");

    if (string.IsNullOrWhiteSpace(dto.Produto))
         erros.Add("Produto é obrigatorio");

    if (dto.Quantidade <=0)
        erros.Add("Quantidade deve ser maior que 0");

    if (dto.Valor <=0)
        erros.Add("Valor deve ser maior que 0");    

    if (erros.Count > 0)
        return Results.BadRequest(new { erros });    

    db.Pedidos.Add(pedido);
    await db.SaveChangesAsync();

    try{
        // Publicamos a mensagem no RabbitMQ    
        var publisher = new RabbitMqPublisher();
        publisher.Publicar(QueueNames.PedidosCriados, pedido);
    }
    catch (Exception ex) {
        app.Logger.LogError(ex , "Falha ao publicar o pedido no RabbitMQ. PedidoId={PedidoId}", pedido.Id);
    }
    // Retornamos status 201 (Created) com o pedido criado
    return Results.Created($"/pedidos/{pedido.Id}", pedido);
});

//
app.MapGet("/pedidos", async (AppDbContext db) =>
{
    //
    var pedidos = await db.Pedidos
    .AsNoTracking()  //
    .OrderByDescending(p => p.CriadoEm)  //
    .ToListAsync();  //

    return Results.Ok(pedidos);

});

app.MapGet("pedidos/{id:guid}", async (Guid id, AppDbContext db) =>
{

    var pedido = await db.Pedidos
    .AsNoTracking()
    .FirstOrDefaultAsync(p=> p.Id == id);

    return pedido is null
    ? Results.NotFound()
    : Results.Ok(pedido);

});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
