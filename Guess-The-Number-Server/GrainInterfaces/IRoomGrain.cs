namespace GrainInterfaces;

public interface IRoomGrain : IGrainWithGuidKey
{
    [Alias("Initialize")] Task Initialize(Guid player1Id,Guid player2Id);
    [Alias("SubmitGuess")] Task SubmitGuess(Guid playerId, int guess);
}