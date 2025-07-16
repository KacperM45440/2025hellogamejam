using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MoneyController : MonoBehaviour
{
    public JarScript JarRef;
    public Image BeeRef;
    public GameFlowController FlowControllerRef;
    public MoneyJarScript jarRef;
    public CashRegisterScript registerRef;
    [SerializeField] public TextMeshProUGUI moneyCounter;
    [SerializeField] public int currentMoney = 0;

    [SerializeField] public int currentMoneyInJar = 0;
    [SerializeField] public int moneyRequiredToWin = 3000;

    private bool jarCashedIn = false;
    private bool moneyCashedIn = false;

    private string controllerName = "MoneyController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

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
        UpdateUI();

        BeeRef.gameObject.GetComponent<Animator>().SetTrigger("BeeSpin");
        moneyCashedIn = true;
        CheckIfAllPlayerCashedIn();
    }

    public void gainMoneyToJar(int amount)
    {
        currentMoneyInJar += amount;
        UpdateUI();
        jarCashedIn = true;
        CheckIfAllPlayerCashedIn();
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

    private void CheckIfAllPlayerCashedIn()
    {
        if(jarCashedIn && moneyCashedIn)
        {
            FlowControllerRef.FinishRequirement(controllerName);
            jarCashedIn = false;
            moneyCashedIn = false;
        }
    }

    public void WaitForPlayerCashIn()
    {
        jarRef.SetJarHungry(true);
        registerRef.SetJarHungry(true);
        jarCashedIn = false;
        moneyCashedIn = false;
    }

    public void HandPickedUpMoney(bool value)
    {
        jarRef.HandPickedUpMoney(value);
        registerRef.HandPickedUpMoney(value);
    }
}
