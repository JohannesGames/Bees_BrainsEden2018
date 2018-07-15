using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenTile : MonoBehaviour
{
    public enum TyleType
    {
        Heavy,
        Light
    }

    public TyleType tyleType;

    public Transform[] flowerPositions;
    Dictionary< int, FlowerPower> placedFlowers = new Dictionary<int, FlowerPower>();
    int numberOfFlowers;
    int positionIndex;

    public void OnEnable()
    {
        numberOfFlowers = Random.Range(0, flowerPositions.Length);

        for (int i = 0; i < numberOfFlowers; i++)
        {
            if (GameManager.gm.flowerPool.pooledFlowers.Count > 0)
            {
                positionIndex = Random.Range(0, flowerPositions.Length);

                if (!placedFlowers.ContainsKey(positionIndex))
                {
                    placedFlowers.Add(positionIndex, GameManager.gm.flowerPool.GetFlower(Random.Range(0, GameManager.gm.flowerPool.pooledFlowers.Count)));
                    placedFlowers[positionIndex].transform.parent = null;
                    placedFlowers[positionIndex].gameObject.SetActive(true);
                }
                else
                {
                    // pick another position
                    i--;
                }
            }
            else
            {
                break;
            }
        }
    }

    void AddToPool()
    {
        GameManager.gm.tilePool.AddToPool(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            // move to tile pool
            Invoke("AddToPool", 2);
            GameManager.gm.gl.SpawnNextTile();
        }
    }
}
