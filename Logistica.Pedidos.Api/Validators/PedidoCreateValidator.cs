using Logistica.Pedidos.Api.Models;

namespace Logistica.Pedidos.Api.Validators;

// Validator = responsabilidade única: validar DTO de entrada
public static class PedidoCrateValidator
{
    // Retorna a lista de erros. Se vier vazia => dto válido.
    public static List<string> Validate(PedidoCreateDto dto)
    {
        var erros = new List<string>();

        // Cliente obrigatório
        if(string.IsNullOrWhiteSpace(dto.Cliente))
        erros.Add("Cliente é obrigatorio.");

        // Produto obrigatório
        if(string.IsNullOrWhiteSpace(dto.Produto))
        erros.Add("Produto é obrigatorio");

        // Quantidade precisa ser maior que 0
        if (dto.Quantidade <= 0)
            erros.Add("Quantidade deve ser maior que zero.");

        // Valor precisa ser maior que 0
        if (dto.Valor <= 0)
            erros.Add("Valor deve ser maior que zero.");

        return erros;
    }
}