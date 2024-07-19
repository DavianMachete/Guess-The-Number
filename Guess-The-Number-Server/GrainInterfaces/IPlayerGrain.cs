namespace GrainInterfaces;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task SetName(string name);
    Task<string> GetName();
    Task JoinQueueAsync();
    Task OnGameStarted(Guid roomId);
    Task OnGameRoundStarted(int roundNumber);
    Task SubmitGuessAsync(int guess);
    Task OnDrawRound(int roundNumber);
    Task OnWinRound(int roundNumber);
    Task OnLoseRound(int roundNumber);
    Task OnWinGameAsync();
    Task OnLoseGameAsync();
}