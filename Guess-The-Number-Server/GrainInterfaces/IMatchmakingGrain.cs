namespace GrainInterfaces;

public interface IMatchmakingGrain : IGrainWithIntegerKey
{
    Task AddPlayerToQueue(string playerId);
}