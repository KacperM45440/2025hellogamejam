using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MoneyController : MonoBehaviour
{
    public JarScript JarRef;
    public Image BeeRef;
    [SerializeField] public TextMeshProUGUI moneyCounter;
    [SerializeField] public int currentMoney = 0;

    [SerializeField] public int currentMoneyInJar = 0;
    [SerializeField] public int moneyRequiredToWin = 3000;

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
        Debug.Log("Gaining money: " + amount.ToString() + "$B");
        currentMoney += amount;
        currentMoneyInJar += amount;
        UpdateUI();

        JarRef.JarGO.gameObject.GetComponent<Animator>().SetTrigger("JarSpin");
        BeeRef.gameObject.GetComponent<Animator>().SetTrigger("BeeSpin");
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
        Debug.Log("Updating money UI: " + currentMoney.ToString() + "$B");
        moneyCounter.text = currentMoney.ToString() + " $B";
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
