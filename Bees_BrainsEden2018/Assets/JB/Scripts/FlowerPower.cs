using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPower : MonoBehaviour
{
    public int publicInt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            // if PC detected
            GameManager.gm.pc.OnNectarPickup();
            GameManager.gm.flowerPool.AddToPool(this);
        }
    }
}
