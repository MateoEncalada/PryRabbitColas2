using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace PryRabbitColas2
{
    class PryRabbitColas2
    {
        static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("TestTopic", ExchangeType.Topic);
                channel.QueueDeclare("notificacionesMail", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind("notificacionesMail", "TestTopic", "notificaciones");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Mensaje recibido en notificacionesMail: {0}", message);
                };

                channel.BasicConsume(queue: "notificacionesMail",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Esperando mensajes. Presiona cualquier tecla para salir.");
                Console.ReadKey();
            }
        }
    }
}
