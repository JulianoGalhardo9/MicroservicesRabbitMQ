using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "order_queue", 
                                durable: false, 
                                exclusive: false, 
                                autoDelete: false, 
                                arguments: null);

string message = "Pedido #123: Smartphone Apple M3";
var body = Encoding.UTF8.GetBytes(message);

// 4. Publicação usando o novo padrão
await channel.BasicPublishAsync(exchange: string.Empty, 
                                routingKey: "order_queue", 
                                body: body);

Console.WriteLine($" [x] Enviado: {message}");