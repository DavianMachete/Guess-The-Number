namespace GrainInterfaces;

public interface IPlayerViewer : IGrainObserver
{
    void GameStarted(string opponentName);
    void RoundStarted(int roundNumber);
    void RoundFinished(Result result, int playerPoints, int opponentPoints);
    void GameFinished(Result result, int playerPoints, int opponentPoints);

}