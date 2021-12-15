using MassTransit;
using Microsoft.Extensions.Logging;

namespace Mass.Sample.Common
{
    public class BroadcastConsumer :
       IConsumer<BroadcastMessage>
    {
        ILogger<BroadcastMessage> _logger;

        public BroadcastConsumer(ILogger<BroadcastMessage> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BroadcastMessage> context)
        {
            _logger.LogInformation("Broadcast Message Value: {Value}", context.Message.Broadcast);
        }
    }
}
