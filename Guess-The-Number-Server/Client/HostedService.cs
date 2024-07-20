using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using GrainInterfaces;

namespace Client;

public sealed partial class HostedService(IClusterClient client, IHostApplicationLifetime applicationLifetime)
    : BackgroundService
{
    private ConsoleViewer? _viewer;
    private IPlayerViewer? _viewerRef;
    private IPlayerGrain? _player;
    
    private const string Greeting = """
                                    
                                    
                                    
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
                                                                                                                                                                                               
                                                                                                                                                                                               
                                                                                                                                                                                               
                                    """;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await AuthenticateAsync();
        
        AnsiConsole.WriteLine(Greeting);
        ShowHelp();

        while (!stoppingToken.IsCancellationRequested)
        {
            var command = Console.ReadLine();
            switch (command)
            {
                case null:
                    continue;
                case "/help":
                    ShowHelp();
                    continue;
                case "/start":
                    await StartAGameAsync();
                    continue;
                case "/quit":
                    applicationLifetime.StopApplication();
                    continue;
                default:
                {
                    var submitted = await TrySubmitGuessAsync(command);
                    if (submitted)
                    {
                        continue;
                    }
                    AnsiConsole.MarkupLine("[bold grey][[[/][bold red]✗[/][bold grey]]][/] [red underline]Unknown command[/][red].[/] Type [bold fuchsia]/help[/] for a list of commands.");
                    continue;
                }
            }
        }
    }

    private async Task AuthenticateAsync()
    {
        AnsiConsole.WriteLine("What's your name?");
        var name = Console.ReadLine()!;

        _player = client.GetGrain<IPlayerGrain>(Guid.NewGuid());
        
        _viewer = new ConsoleViewer(name);
        _viewerRef = client.CreateObjectReference<IPlayerViewer>(_viewer);
        
        await _player.InitializePlayer(name, _viewerRef);
    }

    private async Task StartAGameAsync()
    {
        var matchmaking = client.GetGrain<IMatchmakingGrain>(0);
        await matchmaking.AddPlayerToQueue(_player.GetPrimaryKey());
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Searching for a game....");
    }
    
    private async Task<bool> TrySubmitGuessAsync(string command)
    {
        if (_player is null)
        {
            AnsiConsole.MarkupLine("[bold grey][[[/][bold red]✗[/][bold grey]]][/] [red underline]Player not authenticated.[/][red].");
            return false;
        }
        
        var match = SubmitGuessRegex().Match(command);

        if (!match.Success) 
            return false;
        
        var roomId = await EnsureActiveGameAsync();
        if (roomId == null)
        {
            AnsiConsole.MarkupLine("[bold grey][[[/][bold yellow]✗[/][bold grey]]][/] [yellow underline]Cannot guess " +
                                   "number when player didnt join a game[/][red].[/] Try to join a game by command [bold fuchsia]/start[/] or type " +
                                   "[bold fuchsia]/help[/] for a list of commands.");
            return false;
        }

        var numberString = match.Groups["number"].Value;
        var number = int.Parse(numberString);

        AnsiConsole.MarkupLine("[bold grey][[[/][bold lime]✓[/][bold grey]]][/] Submitting a guess: {0}", number);

        var room = client.GetGrain<IRoomGrain>((Guid)roomId);
        var playerId = _player.GetPrimaryKey();
        await room.SubmitGuess(playerId, number);
        return true;
    }
    
    private async Task<Guid?> EnsureActiveGameAsync()
    {
        if (_player is null)
        {
            AnsiConsole.MarkupLine("[bold grey][[[/][bold red]✗[/][bold grey]]][/][red underline]Player not authenticated.[/]");
            
            return null;
        }

        var currentRoomId = await _player.GetCurrentRoomId();
        return currentRoomId;
    }

    private static void ShowHelp()
    {
        var markup = new Markup("""
                                [bold fuchsia]/help[/]: Shows this [underline green]help[/] text.
                                [bold fuchsia]/start[/]: Starts to find a game.
                                [bold fuchsia]/guess[/] [aqua]<number>[/]: Sends guessed [underline green]number[/].
                                [bold fuchsia]/quit[/]: Closes this client.
                                """);
        
        AnsiConsole.WriteLine();
        AnsiConsole.Write(markup);
        AnsiConsole.WriteLine();
    }
    
    [GeneratedRegex("^/guess (?<number>\\d+)$")]
    private static partial Regex SubmitGuessRegex();
}