using MassTransit;
using Microsoft.Extensions.Logging;

namespace Mass.Sample.Common
{
    public class SampleConsumer :
       IConsumer<SampleMessage>
    {
        ILogger<SampleMessage> _logger;

        public SampleConsumer(ILogger<SampleMessage> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SampleMessage> context)
        {
            _logger.LogInformation("Sample Message Value: {Value}", context.Message.Sample);
        }
    }
}
