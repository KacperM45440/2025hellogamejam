using DG.Tweening;
using UnityEngine;

public class CashRegisterScript : MoneyJarScript
{
    public override void Interact()
    {
        isJarHungry = false;
        interactable = false;
        HandPickedUpMoney(false);
        Debug.Log("Jar interacted with. Hungry: " + isJarHungry);

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

        moneyControllerRef.gainMoney(moneyAmount);
        //animatorRef.SetTrigger("JarSpin"); //Animacja podskakuj¹cej kasy by³¹by fajna        
    }
}
