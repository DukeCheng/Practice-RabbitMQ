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
using RabbitMQ.Producer;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverName = ConfigurationManager.AppSettings["RabbitServer"];
            var queueName = ConfigurationManager.AppSettings["QueueName"];

            //var factory = new ConnectionFactory() { HostName = ConfigurationManager.AppSettings["RabbitServer"] };
            //using (var connection = factory.CreateConnection())
            //{
            //    using (var channel = connection.CreateModel())
            //    {
            //        channel.QueueDeclare(queue: "hello",
            //                    durable: false,
            //                    exclusive: false,
            //                    autoDelete: false,
            //                    arguments: null);

            //        string message = "Hello World!";
            //        var body = Encoding.UTF8.GetBytes(message);

            //        channel.BasicPublish(exchange: "",
            //                             routingKey: "hello",
            //                             basicProperties: null,
            //                             body: body);
            //        Console.WriteLine(" [x] Sent {0}", message);



            //    }
            //}

            var container = new Castle.Windsor.WindsorContainer();
            var adapter = new CastleWindsorContainerAdapter(container);

            container.Register(Classes.FromAssemblyContaining<AuditingLogConsumer>()
           .BasedOn<IHandleMessages>()
           .WithServiceAllInterfaces()
           .LifestyleTransient());

            var bus = Configure.With(adapter)
                .Options(i => i.SetNumberOfWorkers(10).SetMaxParallelism(1))
                .Logging(i => i.NLog())
                .Transport(t => t.UseRabbitMq(serverName, queueName))
                .Start();

            bus.Publish(new TestMessage() { Name = "Duke" });

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    public class TestMessage
    {
        public string Name { get; set; }
    }
}
