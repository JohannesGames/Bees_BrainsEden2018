using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenLoader : MonoBehaviour
{
    public GardenTile[] allGardenTiles;
    public float tileLength = 40;
    public int tilesPerGarden = 10;
    Vector3 spawnLocation = Vector3.zero;
    public GardenDivider[] allGardenDividers;
    public GardenDivider nextDivider;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnFirstGarden()
    {
        spawnLocation.z = GameManager.gm.pc.transform.position.z + tileLength * tilesPerGarden;
        nextDivider = Instantiate(allGardenDividers[Random.Range(0, allGardenDividers.Length)], spawnLocation, Quaternion.identity);
        nextDivider.isNext = true;
    }
}
