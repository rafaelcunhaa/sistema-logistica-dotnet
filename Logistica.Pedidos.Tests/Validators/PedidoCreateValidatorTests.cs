using Logistica.Pedidos.Api.Models;
using Logistica.Pedidos.Api.Validators;
using Xunit;

namespace Logistica.Pedidos.Tests.Validators;

public class PedidoCreateValidatorTests
{
    [Fact]
    public void Deve_retornar_erros_quando_dto_invalido()
    {
        // Arrange
        var dto = new PedidoCreateDto
        {
            Cliente = "",
            Produto = "",
            Quantidade = 0,
            Valor = 0
        };

        // Act
        var erros = Logistica.Pedidos.Api.Validators.PedidoCreateValidator.Validate(dto);


        // Assert
        Assert.NotEmpty(erros);
        Assert.Equal(4, erros.Count);
    }

    [Fact]
    public void Nao_deve_retornar_erros_quando_dto_valido()
    {
        // Arrange
        var dto = new PedidoCreateDto
        {
            Cliente = "Rafael",
            Produto = "Notebook",
            Quantidade = 2,
            Valor = 3500
        };

        // Act
        var erros = Logistica.Pedidos.Api.Validators.PedidoCreateValidator.Validate(dto);

        // Assert
        Assert.Empty(erros);
    }
}
