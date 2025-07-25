using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;


public enum ItemType
{
    FRAME,
    UNDERBARREL,
    GRIP,
    MAGAZINE, 
    STOCK,
    SIGHT,
    BARREL,
    MONEY
}

[Serializable]
public struct ItemAnchor
{
    public ItemAnchorTarget anchor;
    public ItemType[] avaliableTypes;
}


public class Item : MonoBehaviour
{
    public string name;
    public int price;
    public string iconName;
    public ItemType itemType;
    public String description;
    public ItemAnchor[] itemAnchors;
    public SpriteRenderer itemPlaceholder;
    public Transform itemAnchor;
    public Vector3 handGrabPosOffset;
    public Vector3 handGrabRotOffset;
    public List<ItemCharacteristics> characteristics = new List<ItemCharacteristics>();

    [SerializeField] private SpriteRenderer itemSprite;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Rigidbody rb;
    public bool isHovered = false;
    public bool isGrabed;
    public bool inCrafting;

    private SpriteRenderer[] _circles;
    public Item parentItem;
    public Collider itemCollider;
    public TextMeshPro textMeshDescriptionRef;
    public TextMeshPro textMeshNameRef;
    [SerializeField] private Cardboarder cardboarder;


    private void Awake()
    {
        _circles = new SpriteRenderer[itemAnchors.Length];
        for (int i = 0; i < itemAnchors.Length; i++)
        {
            _circles[i] = itemAnchors[i].anchor.circle;
            itemAnchors[i].anchor.parentItem = this;
            itemAnchors[i].anchor.itemTypes = itemAnchors[i].avaliableTypes;
        }
        itemSprite.sortingOrder = (itemType == ItemType.FRAME) ? 0 : 1;
        outline.sortingOrder = -1;
        cardboarder.Initialize(itemSprite);

        ClearCircles();
    }
    


    public void SetHover(bool hover)
    {
        if(hover == isHovered) return;
        if (parentItem)
        {
            if (!parentItem.inCrafting)
            {
                outline.DOFade(0f, 0.5f);
                isHovered = false;
                return;
            }
        }
        isHovered = hover;

        // if (isHovered)
        // {
        //     textMesh.gameObject.SetActive(true);
        // }
        // else
        // {
        //     textMesh.gameObject.SetActive(false);
        // }
         
        textMeshDescriptionRef.text = description;
        textMeshNameRef.text = name;
        outline.color = itemType == ItemType.FRAME ? Color.yellow : Color.white;
        outline.DOFade(isHovered ? 1f : 0f, 0.5f);
    }

    public void SetOutlineColor(Color color)
    {
        outline.color = color;
    }

    public void StartGrab()
    {
        textMeshDescriptionRef.gameObject.SetActive(false);
        textMeshNameRef.gameObject.SetActive(false);
        itemSprite.sortingOrder = 10;
        outline.sortingOrder = 9;

        for (int i = 0; i < itemAnchors.Length; i++)
        {
            if (itemAnchors[i].anchor.addedItem)
            {
                itemAnchors[i].anchor.addedItem.itemSprite.sortingOrder = 10;
                itemAnchors[i].anchor.addedItem.outline.sortingOrder = 9;
                itemAnchors[i].anchor.addedItem.gameObject.layer = LayerMask.NameToLayer("ItemGrabed");

            }
        }
        ClearCircles();
        SetHover(true);
        gameObject.layer = LayerMask.NameToLayer("ItemGrabed");
        rb.isKinematic = true;
        isGrabed = true;
        inCrafting = false;

        if (itemType == ItemType.FRAME && !CraftingMgr.Instance.currentItem)
        {
            itemPlaceholder.transform.parent = CraftingMgr.Instance.craftingAnchor;
            itemPlaceholder.transform.localPosition = -itemAnchor.transform.localPosition;
            itemPlaceholder.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            itemPlaceholder.gameObject.SetActive(true);
            itemPlaceholder.color = new Color(1f, 1f, 1f, 0f);
            itemPlaceholder.DOKill();
            itemPlaceholder.DOFade(1f, 0.25f).SetLoops(-1, LoopType.Yoyo);
        }

        if (parentItem)
        {
            for (int i = 0; i < parentItem.itemAnchors.Length; i++)
            {
                if (parentItem.itemAnchors[i].anchor.addedItem == this)
                {
                    parentItem.itemAnchors[i].anchor.addedItem = null;
                    break;
                }
            }

            parentItem.RefreshCircles(this);
            parentItem = null;
        }
    }

    public void PlaceToCrafting()
    {
        SetHover(false);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        isGrabed = true;
        inCrafting = true;
    }

    public void Drop(Vector3 velocity, bool toCrafting = false)
    {
        SetHover(false);
        if (!toCrafting)
        {        
            itemCollider.isTrigger = false;
            rb.isKinematic = false;
            rb.linearVelocity = velocity;
            transform.parent = null;
            isGrabed = false;
        }

        itemPlaceholder.DOKill();
        itemPlaceholder.transform.parent = transform;
        itemPlaceholder.gameObject.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Item");
        
        
        itemSprite.sortingOrder = (itemType == ItemType.FRAME) ? 0 : 1;
        outline.sortingOrder = -1;

        for (int i = 0; i < itemAnchors.Length; i++)
        {
            if (itemAnchors[i].anchor.addedItem)
            {
                itemAnchors[i].anchor.addedItem.itemSprite.sortingOrder = (itemType == ItemType.FRAME) ? 0 : 1;
                itemAnchors[i].anchor.addedItem.outline.sortingOrder = -1;
                itemAnchors[i].anchor.addedItem.gameObject.layer = LayerMask.NameToLayer("Item");

            }
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone") && !isGrabed)
        {
           
            isGrabed = true;
            transform.DOKill();
            transform.DOScale(0f, 0.25f).OnComplete(() =>
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, 0.2f);
                rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, 0.2f);
                if(itemType == ItemType.MONEY)
                {
                    transform.position = ClientController.Instance.MoneySpawnPoint.position;
                }
                else
                {
                    transform.position = CraftingMgr.Instance.GetItemSpawnPosition();
                }
                transform.eulerAngles =  transform.eulerAngles.With(x: 0, z: 0);
                transform.localScale = Vector3.one;
                isGrabed = false;
            });
           
        }
    }

    public void RefreshCircles(Item handCurrentItem = null)
    {
        for (int i = 0; i < _circles.Length; i++)
        {
            Transform circle = _circles[i].transform;
            if (itemAnchors[i].anchor.addedItem)
            {
                if (circle.localScale.x != 0)
                {
                    circle.DOKill();
                    circle.DOScale(0f, 0.25f).SetEase(Ease.InOutBack);
                    itemAnchors[i].anchor.collider.enabled = false;
                }
                itemAnchors[i].anchor.addedItem.RefreshCircles(handCurrentItem);
            }
            else
            {
                if (circle.localScale.x < 1f)
                {
                    bool select = false;
                    if (handCurrentItem != null)
                    {
                        select = itemAnchors[i].avaliableTypes.Contains(handCurrentItem.itemType);
                    }
                    bool isAvailable = handCurrentItem == null || select;
                    
                    circle.DOKill();
                    circle.DOScale(isAvailable ? (select ? 0.75f : 0.5f) : 0f, 0.25f).SetEase(Ease.InOutBack);
                    itemAnchors[i].anchor.collider.enabled = isAvailable;
                }
            }
        }
    }

    public void ClearCircles()
    {
        for (int i = 0; i < _circles.Length; i++)
        {
            Transform circle = _circles[i].transform;
            circle.DOKill();
            circle.DOScale(0f, 0.25f).SetEase(Ease.InOutBack);
            itemAnchors[i].anchor.collider.enabled = false;
            if (itemAnchors[i].anchor.addedItem)
            {
                itemAnchors[i].anchor.addedItem.ClearCircles();
            }
        }
    }

    public void SetItemToAnchor(ItemAnchorTarget anchor, Item item)
    {
        item.PlaceToCrafting();
        item.itemCollider.isTrigger = true;
        item.transform.parent = anchor.transform;
        item.parentItem = this;
        anchor.addedItem = item;
        item.transform.localPosition = Vector3.zero;
        item.transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast);
        item.transform.DOLocalMove(item.itemAnchor.transform.localPosition, 0.25f).OnComplete(() =>
        {
            CraftingMgr.Instance.currentItem.RefreshCircles();
        });
    }
    
    public void EnableInfoText()
    {
        bool canShow = true;
        for (int i = 0; i < itemAnchors.Length; i++)
        {
            if(itemAnchors[i].anchor.addedItem != null)
            {
                canShow = false;
                break;
            }
        }
        if(!canShow)return;
        textMeshDescriptionRef.gameObject.SetActive(true);
        textMeshDescriptionRef.color = new Color(textMeshDescriptionRef.color.r, textMeshDescriptionRef.color.g, textMeshDescriptionRef.color.b, 0f);
        textMeshDescriptionRef.DOKill();
        textMeshDescriptionRef.DOFade(1f, 0.5f).SetDelay(0.25f);

        textMeshNameRef.gameObject.SetActive(true);
        textMeshNameRef.color = new Color(textMeshNameRef.color.r, textMeshNameRef.color.g, textMeshNameRef.color.b, 0f);
        textMeshNameRef.DOKill();
        textMeshNameRef.DOFade(1f, 0.5f).SetDelay(0.25f);
    }
    
    public void DisableInfoText()
    {
        textMeshDescriptionRef.DOKill(true);
        textMeshDescriptionRef.gameObject.SetActive(false);
        textMeshNameRef.DOKill(true);
        textMeshNameRef.gameObject.SetActive(false);

        // Color color = textMesh.color;
        // color.a = 1f;
        // textMesh.color = color;
        //
        // textMesh.DOKill();
        // DOTween.To(() => textMesh.color.a, a =>
        // {
        //     Color c = textMesh.color;
        //     c.a = a;
        //     textMesh.color = c;
        // }, 0f, 0.15f).OnComplete(() =>
        // {
        //     textMesh.gameObject.SetActive(false);
        // });
    }

    public void SetMoneyValue(int moneyValue)
    {
        if (itemType == ItemType.MONEY)
        {
            price = moneyValue;
            description = "A total of " + moneyValue + " $B";
            transform.DOScale(1f, 0.25f).From(0);
        }
    }
}




