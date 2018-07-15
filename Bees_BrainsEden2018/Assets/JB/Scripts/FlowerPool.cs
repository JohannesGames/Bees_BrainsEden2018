using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPool : MonoBehaviour
{
    public int numberInPool = 10;
    [HideInInspector]
    public List<FlowerPower> pooledFlowers = new List<FlowerPower>();
    FlowerPower selectedFlower;

    public void CreateFlowerPool()
    {
        if (GameManager.gm.gl.allFlowers.Length > 0)
        {
            for (int i = 0; i < numberInPool; i++)
            {
                pooledFlowers.Add(Instantiate(GameManager.gm.gl.allFlowers[Random.Range(0, GameManager.gm.gl.allFlowers.Length)], transform));
                pooledFlowers[i].gameObject.SetActive(false);
            }
        }
    }

    public FlowerPower GetFlower(int randomIndex)
    {
        selectedFlower = pooledFlowers[randomIndex];
        pooledFlowers.RemoveAt(randomIndex);
        return selectedFlower;
    }

    public void AddToPool(FlowerPower _flower)
    {
        _flower.transform.SetParent(transform);
        pooledFlowers.Add(_flower);
        _flower.gameObject.SetActive(false);
    }
}
