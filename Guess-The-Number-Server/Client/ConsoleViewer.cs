using Spectre.Console;
using GrainInterfaces;

namespace Client;

public class ConsoleViewer(string userName) : IPlayerViewer
{
    private readonly string _userName = userName ?? throw new ArgumentNullException(nameof(userName));
    private string _opponentName = "UNKNOWN";
    
    public Task GameStarted(string opponentName)
    {
        _opponentName = opponentName;
        AnsiConsole.MarkupLine($"The game started. Your opponent: [bold yellow]{_opponentName}[/]");
        return Task.CompletedTask;
    }

    public Task RoundStarted(int roundNumber)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "The round {0} started." +
            "\nThe dealer holds a number from 0 to 100. Try to guess the number. " +
            "If your guessed number is closer to the dealer's number than the one" +
            " your opponent guessed, then you win." +
            "\nWrite [bold fuchsia]/guess[/] [aqua]<number>[/]: to send your guess.", roundNumber);
        return Task.CompletedTask;
    }

    public Task OnOpponentGuessed(int opponentGuess)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold yellow]{_opponentName}[/] guessed [bold yellow]{opponentGuess}[/]");
        return Task.CompletedTask;
    }

    public Task GuessSubmitted(int number)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("You submitted [bold aqua]{0}[/] number.", number);
        return Task.CompletedTask;
    }

    public Task OnGuessSubmitFailed(string reason)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold yellow]Failed to submit the guess by reason:[/] {reason}");
        return Task.CompletedTask;
    }

    public Task WaitingForOpponent()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("Waiting for [bold fuchsia]{0}'s[/] guessed number.", _opponentName);
        return Task.CompletedTask;
    }

    public Task RoundFinished(Result result, int dealerNumber, int playerPoints, int opponentPoints)
    {
        string resultText;
        switch (result)
        {
            case Result.Win:
                resultText = "[bold green]Win[/]";
                break;
            case Result.Lose:
                resultText = "[bold red]Lose[/]";
                break;
            case Result.Draw:
            default:
                resultText = "[bold gray]Draw[/]";
                break;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"The round finished. You {resultText}.");
        AnsiConsole.MarkupLine($"The dealer hold [bold yellow]{dealerNumber}[/].");
        AnsiConsole.MarkupLine(
            $"Game Score            |  {_userName} {playerPoints}  |  {_opponentName} {opponentPoints} |.");
        return Task.CompletedTask;
    }

    public Task GameFinished(Result result, int playerPoints, int opponentPoints)
    {
        var resultText = result == Result.Win ? 
            "[bold green]Congratulations, you win!!![/]" : 
            "[bold yellow]You Lose...... :([/]";
        
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine(
            "{0}\nEnd score\t|{1} {2}||{3} {4}|",
            resultText, playerPoints, opponentPoints, _userName, _opponentName);
        AnsiConsole.WriteLine();
        return Task.CompletedTask;
    }
}