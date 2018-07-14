using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenTile : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            // move to tile pool
            GameManager.gm.tilePool.AddToPool(this);
        }
    }
}
