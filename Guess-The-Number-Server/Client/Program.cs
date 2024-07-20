using Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;


Console.Title = "Client";

try
{
    Console.WriteLine("Starting server...");
    await StartClientAsync();
    
    return 0;
}
catch(Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}


static async Task StartClientAsync()
{
    await new HostBuilder()
        .UseOrleansClient(builder =>
        {
            builder.UseLocalhostClustering();
            builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "OrleansGuessTheNumber";
            });
        })
        .ConfigureServices(services =>
        {
            services
                .AddSingleton<IHostedService, HostedService>()
                .Configure<ConsoleLifetimeOptions>(sp => sp.SuppressStatusMessages = true);
        })
        .ConfigureLogging(builder => builder.AddDebug())
        .RunConsoleAsync();
}