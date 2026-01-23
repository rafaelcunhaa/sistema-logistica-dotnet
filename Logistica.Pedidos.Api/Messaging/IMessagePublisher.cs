namespace Logistica.Pedidos.Api.Messaging;

// Contrato para publicar mensagens (permite trocar Rabbit por outro e facilita testes)
public interface IMessagePublisher
{
    void Publicar<T>(string queue, T mensagem);
}