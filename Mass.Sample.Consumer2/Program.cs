using GreenPipes;
using MassTransit;
using Mass.Sample.Common;


var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
services.AddMassTransit(x =>
{
    Guid instanceId = Guid.NewGuid();


    x.AddConsumer<SampleConsumer>();
    x.AddConsumer<BroadcastConsumer>().Endpoint(c => c.InstanceId = instanceId.ToString());

    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
    {
        config.Host(new Uri("rabbitmq://localhost"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });


        config.UseKillSwitch(options => options
                .SetActivationThreshold(10)
                .SetTripThreshold(0.15)
                .SetRestartTimeout(m: 1));


        config.ReceiveEndpoint("sample-event-listener", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 100)); 
            
            e.UseRateLimit(20, TimeSpan.FromSeconds(5));
            e.ConfigureConsumer<SampleConsumer>(provider);
        });

        config.ReceiveEndpoint(System.Environment.MachineName + "-broadcast-event-listener2", e =>
        {
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Interval(2, 100));

            e.UseRateLimit(20, TimeSpan.FromSeconds(5));
            e.ConfigureConsumer<BroadcastConsumer>(provider);
        });
    }));
});

services.AddMassTransitHostedService();


var app = builder.Build();
app.Run();
