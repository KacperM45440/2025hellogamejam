using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyJarScript : InteractableObject
{
    public bool isJarHungry = false;
    public Hand handRef;
    public MoneyController moneyControllerRef;
    [SerializeField] private Collider colliderRef;
    [SerializeField] private Animator animatorRef;
    [SerializeField] private TextMeshProUGUI moneyCountRef;

    private void Start()
    {
        colliderRef.enabled = false;
    }

    public void SetJarHungry(bool value)
    {
        isJarHungry = value;
        interactable = value;
    }

    public override void SetOutline(bool value)
    {
        if (!isJarHungry && value)
        {
            return;
        }
        base.SetOutline(value);
    }

    public void ToggleCollider(bool value)
    {
        colliderRef.enabled = value;
    }

    public void HandPickedUpMoney(bool value)
    {
        if (!isJarHungry && value)
        {
            return;
        }
        ToggleCollider(value);
        base.SetOutline(value);
    }

    public override void Interact()
    {
        isJarHungry = false;
        interactable = false;
        HandPickedUpMoney(false);

        Item moneyItem = handRef.currentItem;
        int moneyAmount = moneyItem.price;
        moneyItem.transform.parent = transform;
        handRef.RemoveCurrentItem();
        moneyControllerRef.HandPickedUpMoney(false);
        moneyItem.transform.DOKill();
        moneyItem.transform.DOMove(transform.position, 0.25f).SetEase(Ease.InOutBack);
        moneyItem.transform.DOScale(0f, 0.5f).OnComplete(() =>
        {
            Destroy(moneyItem.gameObject);
        });

        moneyControllerRef.gainMoneyToJar(moneyAmount);
        animatorRef.SetTrigger("JarSpin");
        moneyCountRef.text = moneyControllerRef.currentMoneyInJar.ToString() + " $B";
        if (moneyControllerRef.currentMoneyInJar >= moneyControllerRef.moneyRequiredToWin)
        {
            moneyCountRef.color = Color.green;
        }
    }
}
