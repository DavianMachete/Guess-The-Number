using Spectre.Console;
using GrainInterfaces;

namespace Client;

public class ConsoleViewer(string userName) : IPlayerViewer
{
    private readonly string _userName = userName ?? throw new ArgumentNullException(nameof(userName));
    private string _opponentName = "UNKNOWN";
    
    public void GameStarted(string opponentName)
    {
        _opponentName = opponentName;
        AnsiConsole.MarkupLine(
            $"The game started. Your opponent: [bold yellow]{opponentName}[/]");
    }

    public void RoundStarted(int roundNumber)
    {
        AnsiConsole.MarkupLine(
            "The round {0} started." +
            "\nThe dealer holds a number from 0 to 100. Try to guess the number. " +
            "If your guessed number is closer to the dealer's number than the one" +
            " your opponent guessed, then you win." +
            "\nWrite [bold fuchsia]/guess[/] [aqua]<number>[/]: to send your guess.", roundNumber);
    }

    public void RoundFinished(Result result, int playerPoints, int opponentPoints)
    {
        string resultText; 
        switch (result)
        {
            case Result.Win:
                resultText = "[/][bold green]Win[/]";
                break;
            case Result.Lose:
                resultText = "[/][bold red]Lose[/]";
                break;
            case Result.Draw:
            default:
                resultText = "[/][bold gray]Draw[/]";
                break;
        }
        
        AnsiConsole.MarkupLine(
            "The round finished. You {0}.\nGame Score\t|{1} {2}||{3} {4}|",
            resultText, playerPoints, opponentPoints, _userName, _opponentName);
    }

    public void GameFinished(Result result, int playerPoints, int opponentPoints)
    {
        var resultText = result == Result.Win ? 
            "[/][bold green]Congratulations, you win!!![/]" : 
            "[/][bold yellow]You Lose...... :([/]";
        
        AnsiConsole.MarkupLine(
            "{0}\nEnd score\t|{1} {2}||{3} {4}|",
            resultText, playerPoints, opponentPoints, _userName, _opponentName);
    }
}