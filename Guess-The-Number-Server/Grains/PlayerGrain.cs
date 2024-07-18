using GrainInterfaces;
using Orleans.Runtime;

namespace Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    private readonly IPersistentState<PlayerModel> _playerModel;

    // ReSharper disable once ConvertToPrimaryConstructor
    public PlayerGrain([PersistentState("playerModel", "Default")] IPersistentState<PlayerModel> playerModel)
    {
        _playerModel = playerModel;
    }

    public async Task JoinQueue()
    {
        var matchmakingGrain = GrainFactory.GetGrain<IMatchmakingGrain>(0);
        await matchmakingGrain.AddPlayerToQueue(this.GetPrimaryKeyString());
    }

    public async Task SubmitGuess(int guess)
    {
        var roomId = this.GetPrimaryKeyString(); // Assuming each player is in one room at a time
        var roomGrain = GrainFactory.GetGrain<IRoomGrain>(Guid.Parse(roomId));
        await roomGrain.SubmitGuess(this.GetPrimaryKeyString(), guess);
    }

    public async Task<int> GetPoints()
    {
        await _playerModel.ReadStateAsync();
        return _playerModel.State.Points;
    }

    public async Task AddPoint()
    {
        await _playerModel.ReadStateAsync();
        _playerModel.State.Points++;
        await _playerModel.WriteStateAsync();
    }

    public async Task SetName(string name)
    {
        _playerModel.State.Name = name;
        await _playerModel.WriteStateAsync();
    }
}
