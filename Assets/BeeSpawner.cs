using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> bees = new List<GameObject>();

    [SerializeField] private float minSpawnDelay = 3f;
    [SerializeField] private float maxSpawnDelay = 6f;

    private void Start()
    {
        StartCoroutine(SpawnBeeWithDelay());
    }

    private IEnumerator SpawnBeeWithDelay()
    {
        yield return new WaitForSeconds(1);
        while (bees.Count > 0)
        {
            SpawnBee();
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    private void SpawnBee()
    {
        int randomBeeIndex = Random.Range(0, bees.Count);
        GameObject newBee = bees[randomBeeIndex];
        newBee.SetActive(true);
        bees.RemoveAt(randomBeeIndex);
    }
}
