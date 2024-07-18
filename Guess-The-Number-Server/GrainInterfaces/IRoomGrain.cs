namespace GrainInterfaces;

public interface IRoomGrain : IGrainWithGuidKey
{
    Task EnterRoom(string playerId);
    Task SubmitGuess(string playerId, int guess);
}