using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NectarPickup : MonoBehaviour
{
    public float checkRatePerSecond = 5;
    public float radius = 5;
    float checkTime;
    Collider[] pcChecker;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time >= checkTime)
        {
            checkTime = Time.time + 1 / checkRatePerSecond;
            pcChecker = Physics.OverlapSphere(transform.position, radius, GameManager.gm.pcLayer);

            if (pcChecker.Length > 0)
            {
                // if PC detected
                GameManager.gm.pc.OnNectarPickup();
                Destroy(gameObject);
            }
        }
    }
}
