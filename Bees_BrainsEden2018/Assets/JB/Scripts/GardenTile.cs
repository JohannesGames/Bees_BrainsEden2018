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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
