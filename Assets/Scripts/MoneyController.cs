using UnityEngine;

public class MoneyController : MonoBehaviour
{

    private int currentMoney = 0;

    public void gainMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log("Money gained: " + amount + ". Current total: " + currentMoney);
    }

    public void spendMoney(int amount)
    {
        if (CheckIfPlayerHasEnough(amount))
        {
            currentMoney -= amount;
            return;
        }
        Debug.LogError("Player doesn't have enough money");
    }

    public bool CheckIfPlayerHasEnough(int cost)
    {
        if(cost > currentMoney)
        {
            return false;
        }
        return true;
    }
}
