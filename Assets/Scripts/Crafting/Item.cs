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
    public bool isGrabed;


    public void SetHover(bool hover)
    {
        if(hover == isHovered) return;
        isHovered = hover;
        outline.DOFade(isHovered ? 1f : 0f, 0.25f);
    }

    public void StartGrab()
    {
        SetHover(true);
        rb.isKinematic = true;
        isGrabed = true;
    }

    public void Drop(Vector3 velocity)
    {
        SetHover(false);
        rb.isKinematic = false;
        rb.linearVelocity = velocity;
        transform.parent = null;
        isGrabed = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone") && !isGrabed)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = CraftingMgr.Instance.GetItemSpawnPosition();
            transform.eulerAngles =  transform.eulerAngles.With(x: 0, z: 0);
        }
    }
}




