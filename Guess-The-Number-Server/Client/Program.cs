using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using GrainInterfaces;
using Orleans.Configuration;

using var host = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(clientBuilder =>
    {
        clientBuilder.UseLocalhostClustering();
        clientBuilder.Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansGuessTheNumber";
        });
    })
    .Build();

await host.StartAsync();

Console.WriteLine("""
                                           /$$      /$$           /$$                                                     /$$                                                
                                          | $$  /$ | $$          | $$                                                    | $$                                                
                                          | $$ /$$$| $$  /$$$$$$ | $$  /$$$$$$$  /$$$$$$  /$$$$$$/$$$$   /$$$$$$        /$$$$$$    /$$$$$$                                   
                                          | $$/$$ $$ $$ /$$__  $$| $$ /$$_____/ /$$__  $$| $$_  $$_  $$ /$$__  $$      |_  $$_/   /$$__  $$                                  
                                          | $$$$_  $$$$| $$$$$$$$| $$| $$      | $$  \ $$| $$ \ $$ \ $$| $$$$$$$$        | $$    | $$  \ $$                                  
                                          | $$$/ \  $$$| $$_____/| $$| $$      | $$  | $$| $$ | $$ | $$| $$_____/        | $$ /$$| $$  | $$                                  
                                          | $$/   \  $$|  $$$$$$$| $$|  $$$$$$$|  $$$$$$/| $$ | $$ | $$|  $$$$$$$        |  $$$$/|  $$$$$$/                                  
                                          |__/     \__/ \_______/|__/ \_______/ \______/ |__/ |__/ |__/ \_______/         \___/   \______/                                   
                                                                                                                                                                             
                                                                                                                                                                             
                                                                                                                                                                             
                    /$$$$$$                                               /$$$$$$$$ /$$                       /$$   /$$                         /$$                          
                   /$$__  $$                                             |__  $$__/| $$                      | $$$ | $$                        | $$                          
                  | $$  \__/ /$$   /$$  /$$$$$$   /$$$$$$$ /$$$$$$$         | $$   | $$$$$$$   /$$$$$$       | $$$$| $$ /$$   /$$ /$$$$$$/$$$$ | $$$$$$$   /$$$$$$   /$$$$$$ 
                  | $$ /$$$$| $$  | $$ /$$__  $$ /$$_____//$$_____/         | $$   | $$__  $$ /$$__  $$      | $$ $$ $$| $$  | $$| $$_  $$_  $$| $$__  $$ /$$__  $$ /$$__  $$
                  | $$|_  $$| $$  | $$| $$$$$$$$|  $$$$$$|  $$$$$$          | $$   | $$  \ $$| $$$$$$$$      | $$  $$$$| $$  | $$| $$ \ $$ \ $$| $$  \ $$| $$$$$$$$| $$  \__/
                  | $$  \ $$| $$  | $$| $$_____/ \____  $$\____  $$         | $$   | $$  | $$| $$_____/      | $$\  $$$| $$  | $$| $$ | $$ | $$| $$  | $$| $$_____/| $$      
                  |  $$$$$$/|  $$$$$$/|  $$$$$$$ /$$$$$$$//$$$$$$$/         | $$   | $$  | $$|  $$$$$$$      | $$ \  $$|  $$$$$$/| $$ | $$ | $$| $$$$$$$/|  $$$$$$$| $$      
                   \______/  \______/  \_______/|_______/|_______/          |__/   |__/  |__/ \_______/      |__/  \__/ \______/ |__/ |__/ |__/|_______/  \_______/|__/      
                                                                                                                                                                             
                                                                                                                                                                             
                                                                                                                                                                             
                  """);

Console.WriteLine();
Console.WriteLine("What's your name?");
var name = Console.ReadLine()!;

var client = host.Services.GetRequiredService<IClusterClient>();
var player = client.GetGrain<IPlayerGrain>(Guid.NewGuid());
await player.SetName(name);

var matchmaking = client.GetGrain<IMatchmakingGrain>(0);
await matchmaking.AddPlayerToQueue(player.GetPrimaryKey());
var playerName = await player.GetName();
Console.WriteLine($"Player {playerName} added to queue.");
