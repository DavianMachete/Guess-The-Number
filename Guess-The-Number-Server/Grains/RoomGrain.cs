using GrainInterfaces;

namespace Grains;

public class RoomGrain : Grain, IRoomGrain
{
    private int _targetNumber;
    private readonly Dictionary<Guid, int> _guesses = new();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _targetNumber = new Random().Next(0, 101); // Server generates a number from 0 to 100
        return base.OnActivateAsync(cancellationToken);
    }

    public async Task EnterRoom(Guid playerId)
    {
        _guesses[playerId] = -1; // Initialize with an invalid guess
        
        // _------------__---_--_-_-_-_-_-__
        // TODO: Notify player to submit guess
        // _-------__---_--_-_-________-_-_-
    }

    public async Task SubmitGuess(Guid playerId, int guess)
    {
        _guesses[playerId] = guess;

        if (_guesses.Values.All(g => g >= 0)) // Check if all players have guessed
        {
            // Determine winner
            var closestPlayer = _guesses
                .MinBy(g => Math.Abs(g.Value - _targetNumber))
                .Key;
            var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(closestPlayer);
            await playerGrain.AddPoint();
        }
    }
}