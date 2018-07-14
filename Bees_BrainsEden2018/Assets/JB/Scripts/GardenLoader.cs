using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenLoader : MonoBehaviour
{
    public GardenTile[] allGardenTiles;
    public float tileLength = 40;
    public float dividerLength = 20;
    public int tilesPerGarden = 10;
    Vector3 spawnLocation = Vector3.zero;
    public GardenDivider[] allGardenDividers;
    public GardenDivider nextDivider;
    GardenTile nextTile;
    int tilesSpawned = 0;

    public void SpawnFirstGarden()
    {
        for (int i = 0; i < tilesPerGarden; i++)
        {
            nextTile = GameManager.gm.tilePool.GetGardenTile(Random.Range(0, GameManager.gm.tilePool.pooledTiles.Count));
            nextTile.transform.position = spawnLocation;
            nextTile.transform.parent = null;
            nextTile.gameObject.SetActive(true);
            spawnLocation.z += tileLength;
        }
        nextDivider = Instantiate(GameManager.gm.dividerPool.GetDivider(Random.Range(0, GameManager.gm.dividerPool.pooledDividers.Count)), spawnLocation, Quaternion.identity);
        nextDivider.transform.parent = null;
        nextDivider.gameObject.SetActive(true);
        spawnLocation.z += dividerLength;
    }

    public void SpawnNextTile()
    {
        if (tilesSpawned < tilesPerGarden)
        {
            tilesSpawned++;
            nextTile = GameManager.gm.tilePool.GetGardenTile(Random.Range(0, GameManager.gm.tilePool.pooledTiles.Count));
            nextTile.transform.position = spawnLocation;
            nextTile.transform.parent = null;
            nextTile.gameObject.SetActive(true);
            spawnLocation.z += tileLength;
        }
        else
        {
            tilesSpawned = 0;
            nextDivider = Instantiate(GameManager.gm.dividerPool.GetDivider(Random.Range(0, GameManager.gm.dividerPool.pooledDividers.Count)), spawnLocation, Quaternion.identity);
            nextDivider.transform.parent = null;
            nextDivider.gameObject.SetActive(true);
            spawnLocation.z += dividerLength;
        }
    }
}
