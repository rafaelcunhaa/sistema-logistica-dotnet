namespace Logistica.Pedidos.Api.Models;


// DTO = objeto usado para receber dados de entrada
public class PedidoCreateDto

{

    public string Cliente { get; set; } = string.Empty;
    public string Produto { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }

}