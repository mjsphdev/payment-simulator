namespace PaymentSimulator.API.Configurations;

public static class NServiceBusExtensions
{
    /// <summary>
    /// Configure and start a send-only NServiceBus endpoint, registers IMessageSession for DI
    /// </summary>
    public static async Task<IServiceCollection> AddSendOnlyNServiceBus(this IServiceCollection services, string endpointName)
    {
        // Endpoint configuration
        var endpointConfiguration = new EndpointConfiguration(endpointName);

        endpointConfiguration.SendOnly(); // only publishes
        endpointConfiguration.UseTransport<LearningTransport>(); // matches Handlers
        endpointConfiguration.UsePersistence<LearningPersistence>(); // optional, demo only
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.EnableInstallers();

        // Start endpoint
        var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        // Register IMessageSession for DI
        services.AddSingleton<IMessageSession>(endpointInstance);

        return services;
    }
}
