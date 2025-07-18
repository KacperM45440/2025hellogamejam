using UnityEngine;

public class ItemAnchorTarget : MonoBehaviour
{
    public Collider collider;
    public SpriteRenderer circle;
    public Item parentItem;
    public Item addedItem;
    [HideInInspector] public ItemType[] itemTypes;

}
