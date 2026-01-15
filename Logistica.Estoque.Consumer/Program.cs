using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Logistica.Shared;



// Configuração para conectar no RabbitMQ (rodando no Docker)
var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};


// Abre conexão
using var connection = factory.CreateConnection();


// Abre canal
using var channel = connection.CreateModel();


// Garante que a fila exista (boa prática)
channel.QueueDeclare(
    queue: QueueNames.PedidosCriados,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);


// Cria o consumidor
var consumer = new EventingBasicConsumer(channel);


// Evento disparado quando uma mensagem chega
consumer.Received += (sender, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);

    Console.WriteLine("Mensagem recebida da fila! ");
    Console.WriteLine(json);

    // Confirma que processou a mensagem (ACK)
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

};


// Começa a consumir a fila
channel.BasicConsume(
    queue: QueueNames.PedidosCriados,
    autoAck: false, // false porque vamos dar ACK manual
    consumer: consumer
);

Console.WriteLine($"Consumindo mensagens da fila '{QueueNames.PedidosCriados}'...");
Console.WriteLine("Pressione ENTER para sair.");
Console.ReadLine();



