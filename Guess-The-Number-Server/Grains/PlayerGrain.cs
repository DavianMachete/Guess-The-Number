using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    private Guid? _currentRoomId;
    private readonly IPersistentState<PlayerModel> _playerModel;
    private readonly ILogger<PlayerGrain> _logger;

    private IPlayerViewer? _viewer;
    
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
    
    public async Task Initialize(string name, IPlayerViewer viewer)
    {
        _viewer = viewer;
        _playerModel.State.UserName = name;
        await _playerModel.WriteStateAsync();
        _logger.LogInformation("{GrainType} {GrainKey} Initialized.", GrainType, GrainKey);
    }

    public async Task<string> GetName()
    {
        await _playerModel.ReadStateAsync();
        return _playerModel.State.UserName;
    }

    public Task OnGameStarted(Guid roomId, string opponentName)
    {
        _viewer?.GameStarted(opponentName);
        _logger.LogInformation("{GrainType} {GrainKey} game started.", GrainType, GrainKey);
        _currentRoomId = roomId;
        return Task.CompletedTask;
    }

    public Task<Guid?> GetCurrentRoomId()
    {
        return Task.FromResult(_currentRoomId);
    }

    public Task OnGameRoundStarted(int roundNumber)
    {
        _viewer?.RoundStarted(roundNumber);
        _logger.LogInformation("{GrainType} {GrainKey} game round: {Round} started.", GrainType, GrainKey, roundNumber);
        return Task.CompletedTask;
    }

    public async Task SubmitGuessAsync(int guess)
    {
        _logger.LogInformation("{GrainType} {GrainKey} submitting guess: {Guess}.", GrainType, GrainKey, guess);
        var roomGrain = GrainFactory.GetGrain<IRoomGrain>((Guid)_currentRoomId!);
        await roomGrain.SubmitGuess(this, guess);
    }

    public Task OnRoundFinished(Result result, int playerPoints, int opponentPoints)
    {
        _viewer?.RoundFinished(result,playerPoints,opponentPoints);
        _logger.LogInformation("{GrainType} {GrainKey} Game round finished with {Result}.", GrainType, GrainKey, result.ToString());
        return Task.CompletedTask;
    }

    public async Task OnGameFinishedAsync(Result result, int playerPoints, int opponentPoints)
    {
        _viewer?.GameFinished(result, playerPoints, opponentPoints);
        _currentRoomId = null;
        await _playerModel.ReadStateAsync();
        if (result == Result.Win)
        {
            _playerModel.State.Wins++;
        }
        else
        {
            _playerModel.State.Loses++;
        }
        await _playerModel.WriteStateAsync();
    }
}