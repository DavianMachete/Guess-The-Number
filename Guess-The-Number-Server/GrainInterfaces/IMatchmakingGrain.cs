namespace GrainInterfaces;

public interface IMatchmakingGrain : IGrainWithIntegerKey
{
    [Alias("AddPlayerToQueue")] Task AddPlayerToQueue(Guid playerId);
}