using FortuneLab.MessageQueue.RabbitMq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQ.Consumer
{
    public interface IEventConsumer
    {

    }

    public abstract class MessageConsumer : IEventConsumer
    {
        protected string MessageQueueName { get; private set; }

        public MessageConsumer(string messageQueueName)
        {
            this.MessageQueueName = messageQueueName;
        }

        public void StartConsume()
        {
            var channel = ConnectionManager.Instance.CurrentModel;
            channel.QueueDeclare(queue: MessageQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += consumer_Received;
            channel.BasicConsume(queue: MessageQueueName, noAck: false, consumer: consumer);

        }
        void consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Consumer(ea, message);
            (sender as IBasicConsumer).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        public abstract void Consumer(BasicDeliverEventArgs e, string message);
    }
}
