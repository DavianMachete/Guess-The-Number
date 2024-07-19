using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Orleans.Configuration;


try
{
    Console.WriteLine("Starting server...");
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
    var hostBuilder = new HostBuilder();
    var iHostBuilder = hostBuilder.UseOrleans(OnHostConfigure);
    var host = iHostBuilder.Build();
    await host.StartAsync();

    return host;
}

static void OnHostConfigure(ISiloBuilder builder)
{
    Console.WriteLine("Configuring host...");
    var iSiloBuilder = builder.UseLocalhostClustering();
    iSiloBuilder = iSiloBuilder.Configure<ClusterOptions>(OnConfigureOptions);
    iSiloBuilder.ConfigureLogging(logging => logging.AddConsole());
    
    Console.WriteLine("Adding Memory Grain Storage...");
    builder.AddMemoryGrainStorage("Default"); 
}

static void OnConfigureOptions(ClusterOptions options)
{
    Console.WriteLine("Configuring host builder options...");
    options.ClusterId = "dev";
    options.ServiceId = "OrleansGuessTheNumber";
}