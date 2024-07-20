namespace GrainInterfaces;

public interface IPlayerGrain : IGrainWithGuidKey
{
    [Alias("InitializePlayer")] Task InitializePlayer(string userName, IPlayerViewer viewer);
    [Alias("GetPlayerName")] Task<string> GetPlayerName();
    [Alias("GetCurrentRoomId")] Task<Guid?> GetCurrentRoomId();
    [Alias("OnGameStarted")] Task OnGameStarted(Guid roomId, string opponentName);
    [Alias("OnGameRoundStarted")] Task OnGameRoundStarted(int roundNumber);
    // [Alias("SubmitGuessAsync")] Task SubmitGuessAsync(int guess);
    [Alias("OnOpponentGuessed")] Task OnOpponentGuessed(int opponentGuess);
    [Alias("OnRoundFinished")] Task OnRoundFinished(Result result, int dealerNumber, int playerPoints, int opponentPoints);
    [Alias("OnGameFinishedAsync")] Task OnGameFinishedAsync(Result result, int playerPoints, int opponentPoints);
}