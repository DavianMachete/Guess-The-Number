using Orleans.Runtime;

using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    private Guid _currentRoomId;
    private readonly IPersistentState<PlayerModel> _playerModel;
    private readonly ILogger<PlayerGrain> _logger;
    
    private static string GrainType => nameof(PlayerGrain);
    private string GrainKey => this.GetPrimaryKeyString();

    
    // ReSharper disable once ConvertToPrimaryConstructor
    public PlayerGrain([PersistentState("playerModel", "Default")] IPersistentState<PlayerModel> playerModel,
        ILogger<PlayerGrain> logger)
    {
        _playerModel = playerModel;
        _logger = logger;
    }
    
    public override Task OnActivateAsync(CancellationToken _)
    {
        _logger.LogInformation("{GrainType} {GrainKey} activated.", GrainType, GrainKey);

        return Task.CompletedTask;
    }

    public async Task JoinQueueAsync()
    {
        _logger.LogInformation("{GrainType} {GrainKey} joining to matchmaking queue.", GrainType, GrainKey);
        
        var matchmakingGrain = GrainFactory.GetGrain<IMatchmakingGrain>(0);
        await matchmakingGrain.AddPlayerToQueue(this.GetPrimaryKey());
    }

    public Task OnGameStarted(Guid roomId)
    {
        _logger.LogInformation("{GrainType} {GrainKey} game started.", GrainType, GrainKey);
        _currentRoomId = roomId;
        return Task.CompletedTask;
    }

    public Task OnGameRoundStarted(int roundNumber)
    {
        _logger.LogInformation("{GrainType} {GrainKey} game round: {Round} started.", GrainType, GrainKey, roundNumber);
        return Task.CompletedTask;
    }

    public async Task SubmitGuessAsync(int guess)
    {
        _logger.LogInformation("{GrainType} {GrainKey} submitting guess: {Guess}.", GrainType, GrainKey, guess);
        var roomGrain = GrainFactory.GetGrain<IRoomGrain>(_currentRoomId);
        await roomGrain.SubmitGuess(this, guess);
    }

    public Task OnDrawRound(int roundNumber)
    {
        _logger.LogInformation("{GrainType} {GrainKey} Game round finished with Draw. Round: {Round}.", GrainType, GrainKey, roundNumber);
        return Task.CompletedTask;
    }

    public Task OnWinRound(int roundNumber)
    {
        _logger.LogInformation("{GrainType} {GrainKey} Game round finished with Win. Round: {Round}.", GrainType, GrainKey, roundNumber);
        return Task.CompletedTask;
    }

    public Task OnLoseRound(int roundNumber)
    {
        _logger.LogInformation("{GrainType} {GrainKey} Game round finished with Lose. Round: {Round}.", GrainType, GrainKey, roundNumber);
        return Task.CompletedTask;
    }

    public async Task OnWinGameAsync()
    {
        await _playerModel.ReadStateAsync();
        _playerModel.State.Wins++;
        await _playerModel.WriteStateAsync();
    }

    public async Task OnLoseGameAsync()
    {
        await _playerModel.ReadStateAsync();
        _playerModel.State.Loses++;
        await _playerModel.WriteStateAsync();
    }

    public async Task SetName(string name)
    {
        _playerModel.State.UserName = name;
        await _playerModel.WriteStateAsync();
    }

    public async Task<string> GetName()
    {
        await _playerModel.ReadStateAsync();
        return _playerModel.State.UserName;
    }
}
