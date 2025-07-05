using System;
using DG.Tweening;
using UnityEngine;


public enum ItemType
{
    FRAME,
    UNDERBARREL,
    GRIP,
    MAGAZINE, 
    STOCK,
    SIGHT,
    BARREL
}

[Serializable]
public struct ItemAnchor
{
    public Transform anchor;
    public ItemType[] avaliableTypes;
}


public class Item : MonoBehaviour
{
    public String name;
    public int price;
    public string iconName;
    public ItemType itemType;
    public String description;
    public ItemAnchor[] itemAnchors;
    public SpriteRenderer itemPlaceholder;
    public Transform itemAnchor;
    public Vector3 handGrabPosOffset;
    public Vector3 handGrabRotOffset;

    [SerializeField] private SpriteRenderer itemSprite;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Rigidbody rb;
    public bool isHovered = false;
    public bool isGrabed;

    private SpriteRenderer[] _circles;
    public Item[] addedItems;


    private void Awake()
    {
        _circles = new SpriteRenderer[itemAnchors.Length];
        for (int i = 0; i < itemAnchors.Length; i++)
        {
            _circles[i] = itemAnchors[i].anchor.GetChild(0).GetComponent<SpriteRenderer>();
        }

        addedItems = new Item[itemAnchors.Length];
        ClearCircles();
    }


    public void SetHover(bool hover)
    {
        if(hover == isHovered) return;
        isHovered = hover;
        outline.color = Color.white;
        outline.DOFade(isHovered ? 1f : 0f, 0.25f);
    }

    public void SetOutlineColor(Color color)
    {
        outline.color = color;
    }

    public void StartGrab()
    {
        ClearCircles();
        SetHover(true);
        gameObject.layer = LayerMask.NameToLayer("ItemGrabed");
        rb.isKinematic = true;
        isGrabed = true;

        if (itemType == ItemType.FRAME)
        {
            itemPlaceholder.transform.parent = CraftingMgr.Instance.craftingAnchor;
            itemPlaceholder.transform.localPosition = -itemAnchor.transform.localPosition;
            itemPlaceholder.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            itemPlaceholder.gameObject.SetActive(true);
            itemPlaceholder.color = new Color(1f, 1f, 1f, 0f);
            itemPlaceholder.DOKill();
            itemPlaceholder.DOFade(1f, 0.25f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void PlaceToCrafting()
    {
        SetHover(false);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        isGrabed = true;
    }

    public void Drop(Vector3 velocity, bool toCrafting = false)
    {
        SetHover(false);
        rb.isKinematic = false;
        if (!toCrafting)
        {        
            rb.linearVelocity = velocity;
            transform.parent = null;
            isGrabed = false;
        }

        itemPlaceholder.DOKill();
        itemPlaceholder.transform.parent = transform;
        itemPlaceholder.gameObject.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Item");

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
                transform.position = CraftingMgr.Instance.GetItemSpawnPosition();
                transform.eulerAngles =  transform.eulerAngles.With(x: 0, z: 0);
                transform.localScale = Vector3.one;
                isGrabed = false;
            });
           
        }
    }

    public void RefreshCircles()
    {
        for (int i = 0; i < _circles.Length; i++)
        {
            Transform circle = _circles[i].transform;

            if (addedItems[i])
            {
                if (circle.localScale.x != 0)
                {
                    circle.DOKill();
                    circle.DOScale(0f, 0.25f).SetEase(Ease.InOutBack);
                }
                addedItems[i].RefreshCircles();
            }
            else
            {
                if (circle.localScale.x < 1f)
                {
                    circle.DOKill();
                    circle.DOScale(0.5f, 0.25f).SetEase(Ease.InOutBack);
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
            if (addedItems[i])
            {
                addedItems[i].ClearCircles();
            }
        }
    }
    
}




