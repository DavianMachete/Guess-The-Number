namespace GrainInterfaces;

public interface IRoomGrain : IGrainWithGuidKey
{
    Task Initialize(Guid player1Id,Guid player2Id);
    Task SubmitGuess(IPlayerGrain player, int guess);
}