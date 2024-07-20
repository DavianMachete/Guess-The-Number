using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Grains;

public class RoomGrain(ILogger<RoomGrain> logger) : Grain, IRoomGrain
{
    private int _currentTargetNumber;
    private int _round;
    
    private Guid _player1Id;
    private Guid _player2Id;
    private int _player1Number;
    private int _player2Number;
    private int _player1Wins;
    private int _player2Wins;

    private const int PointsForWin = 5;

    private static string GrainType => nameof(RoomGrain);
    private Guid GrainKey => this.GetPrimaryKey();


    public async Task Initialize(Guid player1Id, Guid player2Id)
    {
        _player1Id = player1Id;
        _player2Id = player2Id;
        
        await StartGameAsync();
        logger.LogInformation("{GrainType} {GrainKey} Initialized.", GrainType, GrainKey);
    }

    public async Task SubmitGuess(Guid playerId, int guess)
    {
        logger.LogInformation("{GrainType} {GrainKey} Player {playerId} guesses number {Guess}.", GrainType, GrainKey,
            playerId, guess);
        if (playerId != _player2Id)
        {
            _player1Number = guess;
            var player2 = GrainFactory.GetGrain<IPlayerGrain>(_player2Id);
            await player2.OnOpponentGuessed(guess);
        }
        else
        {
            _player2Number = guess;
            var player1 = GrainFactory.GetGrain<IPlayerGrain>(_player1Id);
            await player1.OnOpponentGuessed(guess)!;
        }

        if (_player1Number < 0 || _player2Number < 0)
        {
            var waitingId = playerId == _player2Id ? _player1Id : _player2Id;
            logger.LogInformation("{GrainType} {GrainKey} Waiting for player {WaitingId}.", GrainType, GrainKey,
                waitingId);
            return;
        }

        await DefineRoundWinnerAsync();
        await DefineGameWinnerAsync();
    }

    private async Task DefineRoundWinnerAsync()
    {
        var player1Diff = Math.Abs(_currentTargetNumber - _player1Number);
        var player2Diff = Math.Abs(_currentTargetNumber - _player2Number);
        
        var player1Res = Result.Draw;
        var player2Res = Result.Draw;
        if (player1Diff < player2Diff)
        {
            _player1Wins += 1;
            player1Res = Result.Win;
            player2Res = Result.Lose;
        }
        else if (player1Diff > player2Diff)
        {
            _player2Wins += 1;
            player1Res = Result.Lose;
            player2Res = Result.Win;
        }

        var player1 = GrainFactory.GetGrain<IPlayerGrain>(_player1Id);
        var player2 = GrainFactory.GetGrain<IPlayerGrain>(_player2Id);

        await player1.OnRoundFinished(player1Res, _currentTargetNumber, _player1Wins, _player2Wins);
        await player2.OnRoundFinished(player2Res, _currentTargetNumber, _player2Wins, _player1Wins);
    }
    
    private async Task DefineGameWinnerAsync()
    {
        if (_player1Wins < PointsForWin && _player2Wins < PointsForWin)
        {
            await StartNextRound();
            return;
        }


        var player1Res = _player1Wins == PointsForWin ? Result.Win : Result.Lose;
        var player2Res = _player2Wins == PointsForWin ? Result.Win : Result.Lose;
        
        var player1 = GrainFactory.GetGrain<IPlayerGrain>(_player1Id);
        var player2 = GrainFactory.GetGrain<IPlayerGrain>(_player2Id);
        
        await player1.OnGameFinishedAsync(player1Res, _player1Wins,_player2Wins);
        await player2.OnGameFinishedAsync(player2Res, _player2Wins, _player1Wins);
    }

    private async Task StartGameAsync()
    {
        _round = 0;
        _player1Wins= 0;
        _player2Wins = 0;
        
        var player1 = GrainFactory.GetGrain<IPlayerGrain>(_player1Id);
        var player2 = GrainFactory.GetGrain<IPlayerGrain>(_player2Id);
        
        var player1Name = await player1.GetPlayerName();
        var player2Name = await player2.GetPlayerName();
        
        await player1.OnGameStarted(this.GetPrimaryKey(), player2Name);
        await player2.OnGameStarted(this.GetPrimaryKey(), player1Name);
        
        await StartNextRound();
    }

    private async Task StartNextRound()
    {
        _round += 1;

        logger.LogInformation("{GrainType} {GrainKey} Round {Round} started.", GrainType, GrainKey, _round);
        
        _player1Number = -1;
        _player2Number = -1;
        
        _currentTargetNumber = new Random().Next(0, 101);
        
        var player1 = GrainFactory.GetGrain<IPlayerGrain>(_player1Id);
        var player2 = GrainFactory.GetGrain<IPlayerGrain>(_player2Id);

        await player1.OnGameRoundStarted(_round);
        await player2.OnGameRoundStarted(_round);
    }
}