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

    public async Task<string> GetPlayerName()
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

    // public async Task SubmitGuessAsync(int guess)
    // {
    //     logger.LogInformation("{GrainType} {GrainKey} submitting guess: {Guess}.", GrainType, GrainKey, guess);
    //     if (_currentRoomId is null)
    //     {
    //         logger.LogError("{GrainType} {GrainKey} Current Room is not defined.", GrainType, GrainKey);
    //         return;
    //     }
    //     
    //     var roomGrain = GrainFactory.GetGrain<IRoomGrain>((Guid)_currentRoomId);
    //     var playerId = this.GetPrimaryKey();
    //     await roomGrain.SubmitGuess(playerId, guess);
    //     _viewer?.NumberSubmitted(guess);
    // }

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