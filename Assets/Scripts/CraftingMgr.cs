using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CraftingMgr : Singleton<CraftingMgr>
{
    [SerializeField] private Vector2 clampXItemSpawn;
    [SerializeField] private Vector2 clampZItemSpawn;
    [SerializeField] private float itemSpawnRadius;
    public Transform craftingAnchor;
    public Item currentItem;
    public Hand hand;

    private BoxCollider _craftingCollider;


    public override void Awake()
    {
        base.Awake();
        _craftingCollider = craftingAnchor.GetComponent<BoxCollider>();
    }

    private void Update()
    {
    }
    
    public Vector3 GetItemSpawnPosition()
    {
        Vector3 randomPoint = Random.insideUnitSphere * itemSpawnRadius;
        return (transform.position + new Vector3(
            Mathf.Clamp(randomPoint.x, clampXItemSpawn.x, clampXItemSpawn.y),
            0f,
            Mathf.Clamp(randomPoint.z, clampZItemSpawn.x, clampZItemSpawn.y))).With(y: 5f);
    }

    public void RefreshCollider()
    {
        if (hand.currentItem)
        {
            if (hand.currentItem.itemType == ItemType.FRAME && !currentItem)
            {
                _craftingCollider.enabled = true;
                return;
            }
            
        }
        _craftingCollider.enabled = false;

    }

    public void SetCurrentItem(Item item)
    {
        if(currentItem) return;
        currentItem = item;
        currentItem.PlaceToCrafting();
        currentItem.transform.parent = craftingAnchor;
        currentItem.transform.DOKill();
        currentItem.transform.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 0.25f);
        currentItem.transform.DOLocalMove(-currentItem.itemAnchor.localPosition, 0.25f).OnComplete(() =>
        {
            currentItem.RefreshCircles();

        });
    }

}
