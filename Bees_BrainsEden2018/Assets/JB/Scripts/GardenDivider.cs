using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenDivider : MonoBehaviour
{
    public bool isNext;
    public float checkFrequency = 5;
    public float loadNextGardenAtDistance = 200;

    void Update()
    {
        if (isNext)
        {
            if ((GameManager.gm.pc.transform.position - transform.position).sqrMagnitude < loadNextGardenAtDistance * loadNextGardenAtDistance)
            {
                Debug.Log("Load next garden");
            }
        }
    }
}
