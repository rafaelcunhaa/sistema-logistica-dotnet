using Logistica.Pedidos.Api.Data;
using Logistica.Pedidos.Api.Messaging;
using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


namespace Logistica.Pedidos.Tests.Services;

public class PedidoServiceTests
{
    [Fact]
    public async Task CriarAsync_deve_salvar_no_banco_e_publicar_evento()
    {
        // =========================
        // ARRANGE
        // =========================


        // Banco em memória (não usa SQL Server real)
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        using var db = new AppDbContext(options);


        // Logger fake (não validamos log neste teste)
        var logger = Mock.Of<ILogger<PedidoService>>();


        //  AQUI ESTÁ O MOCK DE MENSAGERIA 
        var publisherMock = new Mock<IMessagePublisher>();

        
        // Service real, com dependências controladas
        var service = new PedidoService(db, logger, publisherMock.Object);

        var dto = new PedidoCreateDto
        {
            Cliente = "Rafael",
            Produto = "Notbook",
            Quantidade = 2,
            Valor = 3500
        };

        // =========================
        // ACT
        // =========================
        var pedido = await service.CriarAsync(dto);

        // =========================
        // ASSERT
        // =========================

        // Verifica se salvou no banco
        var totalPedidos = await db.Pedidos.CountAsync();
        Assert.Equal(1, totalPedidos);

        // Verifica se a mensageria foi chamada (SEM RabbitMQ real)
        publisherMock.Verify(
            p => p.Publicar(
                Logistica.Shared.QueueNames.PedidosCriados,
                It.IsAny<Pedido>()
            ),
            Times.Once
        );

        // Verifica dados básicos
        Assert.Equal("Rafael", pedido.Cliente);
        Assert.Equal("Notbook", pedido.Produto);
        Assert.Equal(2, pedido.Quantidade);
        Assert.Equal(3500, pedido.ValorTotal);

    }
}