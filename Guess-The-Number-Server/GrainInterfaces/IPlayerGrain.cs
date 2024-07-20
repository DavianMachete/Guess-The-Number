namespace GrainInterfaces;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task Initialize(string userName, IPlayerViewer viewer);
    Task<string> GetName();
    Task<Guid?> GetCurrentRoomId();
    Task OnGameStarted(Guid roomId, string opponentName);
    Task OnGameRoundStarted(int roundNumber);
    Task SubmitGuessAsync(int guess);
    Task OnRoundFinished(Result result, int playerPoints, int opponentPoints);
    Task OnGameFinishedAsync(Result result, int playerPoints, int opponentPoints);
}