using Logistica.Pedidos.Api.Data;
using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Messaging;
using Logistica.Shared;
using Microsoft.EntityFrameworkCore;

namespace Logistica.Pedidos.Api.Services;



public class PedidoService : IPedidoService
{
    private readonly IMessagePublisher _publisher; 

    private readonly AppDbContext _db;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(AppDbContext db, ILogger<PedidoService> logger, IMessagePublisher publisher)
    {
        _db = db;
        _logger = logger;
        _publisher = publisher;        
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
        _logger.LogInformation(
         "Pedido salvo no banco. PedidoId={PedidoId} Cliente={Cliente} Produto={Produto} Quantidade={Quantidade} ValorTotal={ValorTotal}",
         pedido.Id, pedido.Cliente, pedido.Produto, pedido.Quantidade, pedido.ValorTotal);


        try
        {
        _publisher.Publicar(Logistica.Shared.QueueNames.PedidosCriados, pedido);

          _logger.LogInformation(
            "Evento publicado no RabbitMQ. Queue={Queue} PedidoId={PedidoId}",
            Logistica.Shared.QueueNames.PedidosCriados, pedido.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Falha ao publicar evento no RabbitMQ. Queue={Queue} PedidoId={PedidoId}",
                Logistica.Shared.QueueNames.PedidosCriados, pedido.Id);
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
