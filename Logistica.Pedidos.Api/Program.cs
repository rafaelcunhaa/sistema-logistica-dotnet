using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Messaging;
using Microsoft.EntityFrameworkCore;
using Logistica.Pedidos.Api.Data;
using System.Linq;
using Logistica.Pedidos.Api.Services;



var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Service (regras de negócio de Pedido)
builder.Services.AddScoped<PedidoService>();

var app = builder.Build();

// Pipeline (Swagger só em Dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();


// Endpoint para criar um novo pedido
app.MapPost("/pedidos", async (PedidoCreateDto dto, AppDbContext db,PedidoService service) => {

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

    // Criamos um pedido a partir dos dados recebidos
    var pedido = await service.CriarAsync(dto);

    // Retornamos status 201 (Created) com o pedido criado
    return Results.Created($"/pedidos/{pedido.Id}", pedido);
});

//
app.MapGet("/pedidos", async (PedidoService service) =>
{
    //
    var pedidos = await service.ListarAsync();
    return Results.Ok(pedidos);

});

app.MapGet("/pedidos/{id:guid}", async (Guid id, PedidoService service) =>
{

    var pedido = await service.BuscarPorIdAsync(id);
    return pedido is null
    ? Results.NotFound()
    : Results.Ok(pedido);

});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
