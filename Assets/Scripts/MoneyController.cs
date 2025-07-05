using TMPro;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyCounter;
    [SerializeField] private int currentMoney = 0;

    public void gainMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    public void spendMoney(int amount)
    {
        if (CheckIfPlayerHasEnough(amount))
        {
            currentMoney -= amount;
            UpdateUI();
            return;
        }
        Debug.LogError("Player doesn't have enough money");
    }

    public void UpdateUI()
    {
        moneyCounter.text = currentMoney.ToString() + "$B";
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
