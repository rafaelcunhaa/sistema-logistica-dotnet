using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Logistica.Shared;

namespace Logistica.Pedidos.Api.Messaging;

// Classe responsável por publicar mensagens no RabbitMQ
public class RabbitMqPublisher : IMessagePublisher
{
    public void Publicar<T>(string queue, T mensagem)
    {
        // Configuração de conexão com o RabbitMQ (rodando no Docker)
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        // Criação da conexão com o RabbitMQ
        using var connection = factory.CreateConnection();
        // Criação do canal de comunicação com o RabbitMQ
        using var channel = connection.CreateModel();

        // Declaração da fila (cria a fila se não existir)
        channel.QueueDeclare(
            queue: QueueNames.PedidosCriados,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Serialização da mensagem para JSON
        var json = JsonSerializer.Serialize(mensagem);
        var body = Encoding.UTF8.GetBytes(json);

        // Publicação da mensagem na fila
        channel.BasicPublish(
            exchange: "",
            routingKey: queue,
            basicProperties: null,
            body: body
        );
    }
}