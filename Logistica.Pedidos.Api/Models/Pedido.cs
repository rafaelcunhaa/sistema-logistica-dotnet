using System;

namespace Logistica.Pedidos.Api.Models;

// classe que representa um pedido do sistema
public class Pedido
{

    public Guid Id { get;set;} = Guid.NewGuid();
    public string Cliente { get;set;} = string.Empty;
    public string Produto { get; set;} = string.Empty;
    public int Quantidade { get; set;}
    public decimal ValorTotal { get;set;}
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

}