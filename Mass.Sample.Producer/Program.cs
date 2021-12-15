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
    publishEndpoint.Publish<SampleMessage>(new SampleMessage() { Sample = Guid.NewGuid().ToString() });
    publishEndpoint.Publish<BroadcastMessage>(new BroadcastMessage() { Broadcast  = Guid.NewGuid().ToString() });
});

app.Run();
