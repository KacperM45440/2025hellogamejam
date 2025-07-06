using TMPro;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyCounter;
    [SerializeField] private int currentMoney = 0;

    [SerializeField] private int currentMoneyInJar = 0;
    [SerializeField] private int moneyRequiredToWin = 2000;

    public void Start()
    {
        if(moneyCounter == null)
        {
            Debug.LogError("Money Counter is not assigned in MoneyController");
            return;
        }
        UpdateUI();
    }

    public void gainMoney(int amount)
    {
        currentMoney += amount;
        currentMoneyInJar += amount;
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

    public bool HasEnoughMoneyInJar()
    {
        if (currentMoneyInJar >= moneyRequiredToWin)
        {
            return true;
        }
        return false;
    }
}
