using GrainInterfaces;

namespace Grains;

public class MatchmakingGrain : Grain, IMatchmakingGrain
{
    private readonly List<string> _playerQueue = new();

    public async Task AddPlayerToQueue(string playerId)
    {
        _playerQueue.Add(playerId);

        if (_playerQueue.Count >= 2)
        {
            var player1 = _playerQueue[0];
            var player2 = _playerQueue[1];

            _playerQueue.RemoveRange(0, 2);

            var newRoomId = Guid.NewGuid();
            var roomGrain = GrainFactory.GetGrain<IRoomGrain>(newRoomId);

            await roomGrain.EnterRoom(player1);
            await roomGrain.EnterRoom(player2);
        }
    }
}