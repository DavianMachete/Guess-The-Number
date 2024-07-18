namespace GrainInterfaces;

public interface IGameGrain : IGrainWithGuidKey
{
    Task StartGame(string player1Id, string player2Id);
}