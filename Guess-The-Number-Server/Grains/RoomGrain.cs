using GrainInterfaces;

namespace Grains;

public class RoomGrain : Grain, IRoomGrain
{
    private int _currentTargetNumber;
    private int _round;
    
    private IPlayerGrain? _player1;
    private IPlayerGrain? _player2;
    private readonly Dictionary<IPlayerGrain, int> _guesses = new();
    private readonly Dictionary<IPlayerGrain, int> _wins = new();

    private const int PointsForWin = 5;
    

    public async Task Initialize(Guid player1Id, Guid player2Id)
    {   
        _player1 = GrainFactory.GetGrain<IPlayerGrain>(player1Id);
        _player2 = GrainFactory.GetGrain<IPlayerGrain>(player2Id);
        
        _wins[_player1] = 0;
        _wins[_player2] = 0;
        
        await StartGameAsync();
    }

    public async Task SubmitGuess(IPlayerGrain player, int guess)
    {
        _guesses[player] = guess;
        if (_guesses.Values.All(g => g < 0))
            return;
        
        await DefineRoundWinnerAsync();
        await DefineGameWinnerAsync();
    }

    private async Task DefineRoundWinnerAsync()
    {
        if (_player1 == null || _player2 == null)
            return;
        
        var player1Wins = _wins[_player1];
        var player2Wins = _wins[_player2];
        
        var player1Guess = _guesses[_player1];
        var player2Guess = _guesses[_player2];

        var player1Diff = Math.Abs(_currentTargetNumber - player1Guess);
        var player2Diff = Math.Abs(_currentTargetNumber - player2Guess);
       
        if (player1Diff < player2Diff)
        {
            _wins[_player1] += 1;
            await _player2.OnRoundFinished(Result.Lose, player2Wins, player1Wins);
            await _player1.OnRoundFinished(Result.Win, player1Wins, player2Wins);
        }
        else if (player1Diff > player2Diff)
        {
            _wins[_player2] += 1;
            await _player2.OnRoundFinished(Result.Win, player2Wins, player1Wins);
            await _player1.OnRoundFinished(Result.Lose, player1Wins, player2Wins);
        }
        else
        {
            await _player2.OnRoundFinished(Result.Draw, player2Wins, player1Wins);
            await _player1.OnRoundFinished(Result.Draw, player1Wins, player2Wins);
        }
    }

    private async Task DefineGameWinnerAsync()
    {
        if (_player1 == null || _player2 == null)
            return;
        
        // Check Game win
        var player1Wins = _wins[_player1];
        var player2Wins = _wins[_player2];

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (player1Wins < PointsForWin && player2Wins < PointsForWin)
        {
            await StartNewRound();
            return;
        }

        // the 1st player won
        if (player1Wins == PointsForWin)
        {
            await _player1.OnGameFinishedAsync(Result.Win, player1Wins, player2Wins);
            await _player2.OnGameFinishedAsync(Result.Lose, player2Wins, player1Wins);
            return;
        }
        
        // the 2nd player won
        await _player1.OnGameFinishedAsync(Result.Lose, player1Wins,player2Wins);
        await _player2.OnGameFinishedAsync(Result.Win, player2Wins, player1Wins);
    }

    private async Task StartGameAsync()
    {
        if (_player1 == null || _player2 == null)
            return;

        var player1Name = await _player1.GetName();
        var player2Name = await _player2.GetName();

        await _player1.OnGameStarted(this.GetPrimaryKey(), player2Name);
        await _player2.OnGameStarted(this.GetPrimaryKey(), player1Name);
        
        await StartNewRound();
    }

    private async Task StartNewRound()
    {
        if (_player1 == null || _player2 == null)
            return;
        
        _round += 1;
        
        _guesses[_player1] = -1;
        _guesses[_player2] = -1;
        
        _currentTargetNumber = new Random().Next(0, 101);

        await _player1.OnGameRoundStarted(_round);
        await _player2.OnGameRoundStarted(_round);
    }
}