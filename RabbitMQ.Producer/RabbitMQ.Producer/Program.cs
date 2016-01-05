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

namespace RabbitMQ.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverName = ConfigurationManager.AppSettings["RabbitServer"];
            var queueName = ConfigurationManager.AppSettings["QueueName"];

            var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitServer"], UserName = "dev", Password = "dev", VirtualHost = "dev_host" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var properties = channel.CreateBasicProperties();
                    //properties.SetPersistent(true);
                    properties.Persistent = true;

                    string inputMessage = null;
                    do
                    {
                        var message = GetMessage(new string[] { inputMessage });
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "task_queue",
                            basicProperties: properties,
                            body: body);
                    } while (!(inputMessage = Console.ReadLine()).Equals("exit", StringComparison.OrdinalIgnoreCase));

                }
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }

    public class TestMessage
    {
        public string Name { get; set; }
    }
}
