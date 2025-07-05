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
    SILENCER
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
    public GameObject itemPlaceholder;
    public Vector3 handGrabPosOffset;
    public Vector3 handGrabRotOffset;

    [SerializeField] private SpriteRenderer itemSprite;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Rigidbody rb;
    public bool isHovered = false;


    public void SetHover(bool hover)
    {
        if(hover == isHovered) return;
        isHovered = hover;
        outline.DOFade(isHovered ? 1f : 0f, 0.25f);
    }

    public void StartDrag()
    {
        SetHover(true);
        rb.isKinematic = true;
    }

    public void Drop()
    {
        SetHover(false);
        rb.isKinematic = false;
        transform.parent = null;
    }

}




