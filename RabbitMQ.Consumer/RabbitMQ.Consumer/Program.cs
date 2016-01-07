using Castle.MicroKernel.Registration;
using RabbitMQ.Client;
using Rebus.CastleWindsor;
using Rebus.Config;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rebus.NLog;
using Rebus.RabbitMq;
using RabbitMQ.Client.Events;
using System.Threading;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
           var serverName = ConfigurationManager.AppSettings["RabbitServer"];
            //var queueName = ConfigurationManager.AppSettings["QueueName"];

            var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitServer"], UserName = "dev", Password = "dev", VirtualHost = "dev_host" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "xa_test_logs", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "xa_test_logs",
                                  routingKey: "");

                Console.WriteLine(" [*] Waiting for logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] {0}", message);
                };
                channel.BasicConsume(queue: queueName,
                                     noAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
