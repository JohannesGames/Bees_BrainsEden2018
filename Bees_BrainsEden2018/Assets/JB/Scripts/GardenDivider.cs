using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenDivider : MonoBehaviour
{
    public float checkFrequency = 5;
    public float loadNextGardenAtDistance = 200;

    void AddToPool()
    {
        GameManager.gm.dividerPool.AddToPool(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            // move to divider pool
            Invoke("AddToPool", 2);
            GameManager.gm.gl.SpawnNextTile();
        }
    }
}
