using Logistica.Pedidos.Api.Models;

namespace Logistica.Pedidos.Api.Services;

// Interface = "contrato" do que o serviço de pedidos precisa oferecer
public interface IPedidoService
{
    Task<Pedido> CriarAsync(PedidoCreateDto dto);
    Task<List<Pedido>> ListarAsync();
    Task<Pedido?> BuscarPorIdAsync(Guid id);
}