using MassTransit;
using System;

namespace PocLogRabbitMq.Rabbitmq.Consumer
{
    public class CijunMessageConsumerDefinition : ConsumerDefinition<CijunMessageConsumer>
    {
        private static string EndpointDescription => "fila-api-teste";
        private static int RetryLimit => 3;
        public static int RetryInitialIntervalSeconds => 10;
        public static int RetryIntervalIncrementSeconds => 10;
        public CijunMessageConsumerDefinition()
        {
            EndpointName = EndpointDescription;
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CijunMessageConsumer> consumerConfigurator, IRegistrationContext context)
        {
            consumerConfigurator.UseMessageRetry(r =>
            {
                r.Incremental(RetryLimit, TimeSpan.FromSeconds(RetryInitialIntervalSeconds), TimeSpan.FromSeconds(RetryIntervalIncrementSeconds));
            });
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        }
    }
}