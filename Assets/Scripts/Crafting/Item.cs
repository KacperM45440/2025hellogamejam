using System;
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
    public String description;
}


public class Item : MonoBehaviour
{
    public ItemType itemType;
    public ItemAnchor[] itemAnchors;
    public GameObject itemPlaceholder;
}
