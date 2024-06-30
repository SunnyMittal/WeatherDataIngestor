using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Add queue client for injection in WeatherDataService background service
        services.AddAzureClients(builder =>
        {
            builder.AddClient<QueueClient, QueueClientOptions>((options, _, _) =>
            {
                options.MessageEncoding = QueueMessageEncoding.Base64;
                var credentials = new DefaultAzureCredential();
                var queueUri = new Uri("https://youtubestorageaccount.queue.core.windows.net/youtube");
                return new QueueClient(queueUri, credentials, options);
            });
        });
    })
    .Build();

host.Run();
