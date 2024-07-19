using GrainInterfaces;

namespace Grains;

public class MatchmakingGrain : Grain, IMatchmakingGrain
{
    private readonly List<Guid> _playerQueue = new();

    public async Task AddPlayerToQueue(Guid playerId)
    {
        _playerQueue.Add(playerId);

        if (_playerQueue.Count >= 2)
        {
            var player1 = _playerQueue[0];
            var player2 = _playerQueue[1];

            _playerQueue.RemoveRange(0, 2);

            var newRoomId = Guid.NewGuid();
            var roomGrain = GrainFactory.GetGrain<IRoomGrain>(newRoomId);

            await roomGrain.Initialize(player1, player2);
            
        }
    }
}