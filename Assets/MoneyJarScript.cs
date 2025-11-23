using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MoneyJarScript : InteractableObject
{
    public bool isJarHungry = false;
    public Hand handRef;
    public MoneyController moneyControllerRef;
    public MeshFilter CashMesh;
    public Mesh[] CashMeshes;
    [SerializeField] private Collider colliderRef;
    [SerializeField] private Animator animatorRef;
    [SerializeField] private TextMeshProUGUI moneyCountRef;

    private void Start()
    {
        //colliderRef.enabled = false;
    }

    public void SetJarHungry(bool value)
    {
        isJarHungry = value;
        interactable = value;
    }

    public override void SetOutline(bool value)
    {
        base.SetOutline(value);
        ShowMoney(value);
    }

    public virtual void ShowMoney(bool value)
    {
        if (!value)
        {
            moneyCountRef.text = "";
            return;
        }

        string money = moneyControllerRef.currentMoneyInJar.ToString() + " $B";
        moneyCountRef.text = money;
    }

    public void EnableCollider()
    {
        colliderRef.enabled = true;
    }

    public void DisableCollider()
    {
        colliderRef.enabled = false;
    }

    public void HandPickedUpMoney(bool value)
    {
        if (!isJarHungry && value)
        {
            return;
        }

        if (value)
        {
            EnableCollider();
        }
        else
        {
            DisableCollider();
        }

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
        //moneyCountRef.text = moneyControllerRef.currentMoneyInJar.ToString() + " $B";
        
        int currentMoney = moneyControllerRef.currentMoneyInJar;
        int targetMoney = moneyControllerRef.moneyRequiredToWin;

        Debug.Log(currentMoney + " : " + targetMoney);
        
        if (currentMoney < 0.5f * targetMoney)
        {
            CashMesh.mesh = CashMeshes[0];
        }
        else if (currentMoney < targetMoney)
        {
            CashMesh.mesh = CashMeshes[1];
        }
        else
        {
            CashMesh.mesh = CashMeshes[2];
            moneyCountRef.color = Color.green;
        }
    }
}
