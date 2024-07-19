namespace GrainInterfaces;

public interface IRoomGrain : IGrainWithGuidKey
{
    Task EnterRoom(Guid playerId);
    Task SubmitGuess(Guid playerId, int guess);
}