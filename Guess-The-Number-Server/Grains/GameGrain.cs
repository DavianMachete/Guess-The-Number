using GrainInterfaces;

namespace Grains;

public class GameGrain : Grain, IGameGrain
{
    private string _player1Id;
    private string _player2Id;

    public Task StartGame(string player1Id, string player2Id)
    {
        _player1Id = player1Id;
        _player2Id = player2Id;

        // Initialize game state, notify players, etc.
        return Task.CompletedTask;
    }

    // TODO: Add methods to manage game state, handle player actions, etc.
}