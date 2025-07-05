using UnityEngine;

public class StageManager : MonoBehaviour
{
    public enum GameStage
    {
        EnterStore,
        Greeting,
        Request,
        Workshop,
        Response,
        LeaveStore,
        Newspaper
    }

    private GameStage currentGameStage;

    private void Start()
    {
        currentGameStage = GameStage.EnterStore;
    }

    public GameStage GetCurrentGameStage()
    {
        return currentGameStage;
    }

    public void SetCurrentGameStage(GameStage givenStage)
    {
        currentGameStage = givenStage;
    }
}
