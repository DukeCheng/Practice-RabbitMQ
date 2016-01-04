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

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverName = ConfigurationManager.AppSettings["RabbitServer"];
            var queueName = ConfigurationManager.AppSettings["QueueName"];

            var container = new Castle.Windsor.WindsorContainer();
            var adapter = new CastleWindsorContainerAdapter(container);

            container.Register(Classes.FromAssemblyContaining<AuditLogConsumer>()
           .BasedOn<IHandleMessages>()
           .WithServiceAllInterfaces()
           .LifestyleTransient());

            var bus = Configure.With(adapter)
                .Options(i => i.SetNumberOfWorkers(10).SetMaxParallelism(1))
                .Logging(i => i.NLog())
                .Transport(t => t.UseRabbitMq(serverName, queueName))
                .Start();

            bus.Subscribe<TestMessage>();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
