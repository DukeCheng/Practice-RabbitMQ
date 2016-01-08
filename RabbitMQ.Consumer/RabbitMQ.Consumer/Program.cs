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
using FortuneLab.MessageQueue.RabbitMq;
using Newtonsoft.Json;

namespace RabbitMQ.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {

            //var userLoginConsumer = new UserLoginEventConsumer();
            //userLoginConsumer.StartConsume();

            MessageQueueConsumer.Consume("UserSessionValidationQueue", (ea, message) =>
            {
                Console.WriteLine(" [x] Received {0}", message);
                Console.WriteLine(" [x] Done");
            });

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    public class MessageQueueConsumer
    {
        public static void Consume(string MessageQueueName, Action<BasicDeliverEventArgs, string> messageHandler)
        {
            var channel = ConnectionManager.Instance.CurrentModel;
            channel.QueueDeclare(queue: MessageQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                messageHandler(ea, message);
                (sender as IBasicConsumer).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: MessageQueueName, noAck: false, consumer: consumer);
        }

        public static void Consume<TEventData>(string MessageQueueName, Action<BasicDeliverEventArgs, TEventData> messageHandler)
        {
            var channel = ConnectionManager.Instance.CurrentModel;
            channel.QueueDeclare(queue: MessageQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                messageHandler(ea, JsonConvert.DeserializeObject<TEventData>(message));
                (sender as IBasicConsumer).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: MessageQueueName, noAck: false, consumer: consumer);
        }
    }


    public class UserLoginEventConsumer : MessageConsumer
    {
        public UserLoginEventConsumer()
            : base("UserSessionValidationQueue")
        { }

        public override void Consumer(BasicDeliverEventArgs e, string message)
        {
            Console.WriteLine(" [x] Received {0}", message);

            Console.WriteLine(" [x] Done");
        }
    }

    public class EventConsumer : MessageConsumer
    {
        public EventConsumer(string messageQueueName, Action<BasicDeliverEventArgs, string> messageHandler)
            : base("UserSessionValidationQueue")
        {

        }

        public override void Consumer(BasicDeliverEventArgs e, string message)
        {
            Console.WriteLine(" [x] Received {0}", message);

            Console.WriteLine(" [x] Done");
        }
    }
}
