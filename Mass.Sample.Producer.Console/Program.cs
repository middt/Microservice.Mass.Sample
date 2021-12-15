// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Mass.Sample.Common;

Microsoft.Extensions.Hosting.IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
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
        }))
    .Build();

await host.RunAsync();

using (var scope = host.Services.CreateScope())
{
    IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

    Parallel.For(0, 10, new ParallelOptions() { MaxDegreeOfParallelism = 10 },
    index =>
    {
        publishEndpoint.Publish<SampleMessage>(new SampleMessage() { Sample = "Sample " + index });
        publishEndpoint.Publish<BroadcastMessage>(new BroadcastMessage() { Broadcast = "Broadcast " + index });
    });
}



Console.WriteLine("Hello, World!");
