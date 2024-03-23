using MassTransit;
using Serilog;
using System.Threading.Tasks;

namespace PocLogRabbitMq.Rabbitmq.Consumer
{
    public class CijunMessageConsumer : IConsumer<CijunMessage>
    {
        public Task Consume(ConsumeContext<CijunMessage> context)
        {
            Log.Information("Received message: {Text}", context.Message.Text);
            return Task.CompletedTask;
        }
    }
}