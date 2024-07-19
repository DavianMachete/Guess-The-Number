namespace GrainInterfaces;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task JoinQueue();
    Task SubmitGuess(int guess);
    Task<int> GetPoints();
    Task AddPoint();
    Task SetName(string name);
    Task<string> GetName();
}