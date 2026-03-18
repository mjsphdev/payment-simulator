using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.UseNServiceBus(context =>
{
    var endpointConfiguration = new EndpointConfiguration("PaymentSimulator.ServiceBus");

    endpointConfiguration.UseTransport<LearningTransport>();
    endpointConfiguration.UsePersistence<LearningPersistence>();
    endpointConfiguration.SendFailedMessagesTo("error");
    endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    endpointConfiguration.EnableInstallers();

    return endpointConfiguration;
});

var host = builder.Build();
await host.RunAsync();
