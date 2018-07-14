using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager gm;

    public MovementPC pc;

    public Cinemachine.CinemachineVirtualCamera playerCam;

    // Boundaries
    [Header("Boundaries")]
    public float boundaryTop;
    public float boundaryBottom;
    public float boundaryLeft;
    public float boundaryRight;
    public bool isOutOfBounds;

    void Awake()
    {
        if (gm)
        {
            Destroy(gameObject);
        }
        else
        {
            gm = this;
        }
    }


    void Update()
    {
        CheckBoundaries();
    }

    public void Respawn()
    {
        // TODO add respawn function
    }

    void CheckBoundaries()
    {
        if (!isOutOfBounds)
        {
            if (pc.transform.position.y > boundaryTop)
            {
                // is too high up
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnTop());
                pc.down = true;
                pc.up = false;
                Debug.Log("Moving down");
            }
            else if (pc.transform.position.y < boundaryBottom)
            {
                // is too far down
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnBottom());
                pc.up = true;
                pc.down = false;
                Debug.Log("Moving up");
            }

            if (pc.transform.position.x < boundaryLeft)
            {
                // is too far left
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnLeft());
                pc.left = false;
                pc.right = true;
                Debug.Log("Moving right");
            }
            else if (pc.transform.position.x > boundaryRight)
            {
                // is too far right
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnRight());
                pc.left = true;
                pc.right = false;
                Debug.Log("Moving left");
            }
        }
    }
}
