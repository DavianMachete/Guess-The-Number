using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Grains;

public class PlayerGrain(
    [PersistentState("playerModel", "Default")] IPersistentState<PlayerModel> playerModel,
    ILogger<PlayerGrain> logger)
    : Grain, IPlayerGrain
{
    private Guid? _currentRoomId;

    private IPlayerViewer? _viewer;
    
    private static string GrainType => nameof(PlayerGrain);
    private Guid GrainKey => this.GetPrimaryKey();


    public override Task OnActivateAsync(CancellationToken _)
    {
        logger.LogInformation("{GrainType} {GrainKey} activated.", GrainType, GrainKey);

        return Task.CompletedTask;
    }
    
    public async Task InitializePlayer(string name, IPlayerViewer viewer)
    {
        _viewer = viewer;
        playerModel.State.UserName = name;
        await playerModel.WriteStateAsync();
        logger.LogInformation("{GrainType} {GrainKey} Initialized.", GrainType, GrainKey);
    }

    public async Task<string> GetPlayerNameAsync()
    {
        await playerModel.ReadStateAsync();
        return playerModel.State.UserName;
    }

    public Task OnGameStarted(Guid roomId, string opponentName)
    {
        _viewer?.GameStarted(opponentName);
        logger.LogInformation("{GrainType} {GrainKey} game started.", GrainType, GrainKey);
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
        logger.LogInformation("{GrainType} {GrainKey} game round: {Round} started.", GrainType, GrainKey, roundNumber);
        return Task.CompletedTask;
    }

    public Task OnSubmitGuessFailed(string reason)
    {
        logger.LogInformation("{GrainType} {GrainKey} Failed submit the guess. Reason: {Reason}", GrainType, GrainKey, reason);
        _viewer?.OnGuessSubmitFailed(reason);
        return Task.CompletedTask;
    }

    public Task OnOpponentGuessed(int opponentGuess)
    {
        _viewer?.OnOpponentGuessed(opponentGuess);
        return Task.CompletedTask;
    }

    public Task OnRoundFinished(Result result, int dealerNumber, int playerPoints, int opponentPoints)
    {
        _viewer?.RoundFinished(result, dealerNumber, playerPoints, opponentPoints);
        logger.LogInformation("{GrainType} {GrainKey} Game round finished with {Result}.", GrainType, GrainKey, result.ToString());
        return Task.CompletedTask;
    }

    public async Task OnGameFinishedAsync(Result result, int playerPoints, int opponentPoints)
    {
        _viewer?.GameFinished(result, playerPoints, opponentPoints);
        await playerModel.ReadStateAsync();
        if (result == Result.Win)
        {
            playerModel.State.Wins++;
        }
        else
        {
            playerModel.State.Loses++;
        }
        await playerModel.WriteStateAsync();
    }
}