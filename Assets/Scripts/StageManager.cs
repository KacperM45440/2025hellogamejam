using UnityEngine;

public class StageManager : MonoBehaviour
{
    public enum GameStage
    {
        StartDay,
        ClientEnterStore,
        ClientGreeting,
        Rope,
        ClientRequest,
        ClientWaitForGun,
        ClientGunReview,
        ClientLeaveStore,
        Tablet,
        FinishDay
    }

    private GameStage currentGameStage;

    public GameStage GetCurrentGameStage()
    {
        return currentGameStage;
    }

    public void SetCurrentGameStage(GameStage givenStage)
    {
        currentGameStage = givenStage;
    }
}
