namespace GrainInterfaces;

public interface IMatchmakingGrain : IGrainWithIntegerKey
{
    Task AddPlayerToQueue(Guid playerId);
}