using UnityEngine;

public class CraftingMgr : Singleton<CraftingMgr>
{
    [SerializeField] private Vector2 clampXItemSpawn;
    [SerializeField] private Vector2 clampZItemSpawn;
    [SerializeField] private float itemSpawnRadius;

    public Vector3 GetItemSpawnPosition()
    {
        Vector3 randomPoint = Random.insideUnitSphere * itemSpawnRadius;
        return (transform.position + new Vector3(
            Mathf.Clamp(randomPoint.x, clampXItemSpawn.x, clampXItemSpawn.y),
            0f,
            Mathf.Clamp(randomPoint.z, clampZItemSpawn.x, clampZItemSpawn.y))).With(y: 5f);
    }
}
