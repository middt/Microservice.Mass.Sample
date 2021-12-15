using Mass.Sample.Common;
using MassTransit;


var builder = WebApplication.CreateBuilder(args);


IServiceCollection services = builder.Services;
services.AddMassTransit(x =>
{

    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
    {
        config.Host(new Uri("rabbitmq://localhost"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    }));
});

services.AddMassTransitHostedService();



var app = builder.Build();
app.MapGet("/SendMessage", (IPublishEndpoint publishEndpoint) =>
{
    Parallel.For(0, 10, new ParallelOptions() { MaxDegreeOfParallelism = 10 },
    index =>
    {
        publishEndpoint.Publish<SampleMessage>(new SampleMessage() { Sample = "Sample "+index });
        publishEndpoint.Publish<BroadcastMessage>(new BroadcastMessage() { Broadcast = "Broadcast "+index });
    });
});

app.Run();
