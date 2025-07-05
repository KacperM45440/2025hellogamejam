using UnityEngine;

public class StageManager : MonoBehaviour
{
    public enum GameStage
    {
        Menu,
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
        currentGameStage = GameStage.Menu;
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
