namespace GrainInterfaces;

public interface IPlayerViewer : IGrainObserver
{
    [Alias("GameStarted")] Task GameStarted(string opponentName);
    [Alias("RoundStarted")] Task RoundStarted(int roundNumber);
    [Alias("OnOpponentGuessed")] Task OnOpponentGuessed(int opponentGuess);
    [Alias("GuessSubmitted")] Task GuessSubmitted(int number);
    [Alias("OnGuessSubmitFailed")] Task OnGuessSubmitFailed(string reason);
    [Alias("WaitingForOpponent")] Task WaitingForOpponent();
    [Alias("RoundFinished")] Task RoundFinished(Result result,int dealerNumber, int playerPoints, int opponentPoints);
    [Alias("GameFinished")] Task GameFinished(Result result, int playerPoints, int opponentPoints);
}