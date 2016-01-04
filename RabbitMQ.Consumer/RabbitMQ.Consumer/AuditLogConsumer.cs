using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQ.Consumer
{
    public class AuditLogConsumer : IHandleMessages<TestMessage>
    {
        public System.Threading.Tasks.Task Handle(TestMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessage
    {
        public string Name { get; set; }
    }
}
