using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private TabletController tabletControllerRef;
    [SerializeField] private Image imageField;
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI initialPriceField;
    [SerializeField] private GameObject discountParent;
    [SerializeField] private TextMeshProUGUI discountPriceField;
    [SerializeField] private TextMeshProUGUI orderCountField;
    [SerializeField] [Range(0, 100)] private int discountChance = 0;
    [SerializeField] [Range(1, 99)] private int discountModifierMin = 20;
    [SerializeField] [Range(1, 99)] private int discountModifierMax = 80;
    [SerializeField] private Image frame;
    [SerializeField] private Sprite Sprite;
    private GameObject thisWeaponGO;
    private int currentOrderCount = 0;
    private int currentPrice = 0;

    public StoreItem InitializeItem(GameObject weaponGO, string imageResourcesRef, string nameText, string descriptionText, int initialPrice, TabletController controllerRef)
    {
        imageField.sprite = Resources.Load<Sprite>("WeaponIcons/"+imageResourcesRef);
        nameField.text = nameText;
        descriptionField.text = descriptionText;
        initialPriceField.text = initialPrice.ToString() + " $B";
        tabletControllerRef = controllerRef;
        currentPrice = initialPrice;
        thisWeaponGO = weaponGO;
        RandomizeDiscount();
        return this;
    }

    public void OrderMore()
    {
        if (currentOrderCount >= 9)
        {
            return;
        }
        currentOrderCount++;
        orderCountField.text = currentOrderCount.ToString();
        tabletControllerRef.UpdateTotalPrice(currentPrice);
    }

    public void OrderLess()
    {
        if (currentOrderCount <= 0)
        {
            return;
        }
        currentOrderCount--;
        orderCountField.text = currentOrderCount.ToString();
        tabletControllerRef.UpdateTotalPrice(-currentPrice);
    }

    public void GetOrderCount(out GameObject itemGO, out int returnCount)
    {
        itemGO = thisWeaponGO;
        returnCount = currentOrderCount;
    }

    public void ResetOrderCount()
    {
        int orderPrice = currentPrice * currentOrderCount;
        currentOrderCount = 0;
        orderCountField.text = currentOrderCount.ToString();
        tabletControllerRef.UpdateTotalPrice(-orderPrice);
    }

    private void RandomizeDiscount()
    {
        int randomDiscount = Random.Range(1, 101);
        if (randomDiscount >= discountChance)
        {
            return;
        }
        discountParent.SetActive(true);
        frame.GetComponent<Image>().sprite = Sprite;
        float discountModifier = Random.Range(discountModifierMin, discountModifierMax + 1);
        int discountedPrice = Mathf.RoundToInt(currentPrice * (discountModifier / 100f));
        discountPriceField.text = discountedPrice.ToString() + " $B";
        currentPrice = discountedPrice;
    }
}