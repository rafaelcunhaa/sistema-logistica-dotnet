using Logistica.Pedidos.Api.Data;
using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Messaging;
using Logistica.Shared;
using Microsoft.EntityFrameworkCore;

namespace Logistica.Pedidos.Api.Services;



public class PedidoService
{

    private readonly AppDbContext _db;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(AppDbContext db, ILogger<PedidoService> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<Pedido> CriarAsync(PedidoCreateDto dto)
    {
        var pedido = new Pedido
        {
            Cliente = dto.Cliente,
            Produto = dto.Produto,
            Quantidade = dto.Quantidade,
            ValorTotal = dto.Valor,
            CriadoEm = DateTime.UtcNow
        };
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();

        try
        {
            var publisher = new RabbitMqPublisher();
            publisher.Publicar(Logistica.Shared.QueueNames.PedidosCriados, pedido);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Falha ao publicar pedido no RabbitMQ. PedidoId={PedidoId}", pedido.Id);
        }

        return pedido;
    }

    public async Task<List<Pedido>> ListarAsync()
    {
        return await _db.Pedidos
        .AsNoTracking()
        .OrderByDescending(p=> p.CriadoEm)
        .ToListAsync();
    }

    public async Task<Pedido?> BuscarPorIdAsync(Guid id)
    {
        return await _db.Pedidos
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);
    }

}
