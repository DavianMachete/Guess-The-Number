using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;


try
{
    var host = await StartServer();
    
    Console.WriteLine("Press Enter to terminate...");
    Console.ReadLine();
        
    await host.StopAsync();
    return 0;
}
catch(Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}

static async Task<IHost> StartServer()
{
    var host = new HostBuilder()
        .UseOrleans(builder =>
        {
            builder.UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansGuessTheNumber";
                })
                .ConfigureLogging(logging => logging.AddConsole());
        })
        .Build();
        
    await host.StartAsync();

    return host;
}