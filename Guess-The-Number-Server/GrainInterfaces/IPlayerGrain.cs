namespace GrainInterfaces;

public interface IPlayerGrain : IGrainWithStringKey
{
    Task JoinQueue();
    Task SubmitGuess(int guess);
    Task<int> GetPoints();
    Task AddPoint();
    Task SetName(string name);
}