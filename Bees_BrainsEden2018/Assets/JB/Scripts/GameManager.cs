using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public enum GameState
{
    StartScreen,
    InGame,
    EndScreen
}

public class GameManager : MonoBehaviour
{

    public static GameManager gm;

    public MovementPC pc;

    public Cinemachine.CinemachineVirtualCamera playerCam;
    public Cinemachine.CinemachineVirtualCamera startCam;
    public bool playingTimeline;
    public PlayableDirector startTimeline;

    public LayerMask pcLayer;

    public GardenLoader gl;

    public GardenTilePool tilePool;
    public GardenDividerPool dividerPool;
    public FlowerPool flowerPool;

    public GameState gameState;

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

    private void Start()
    {
        tilePool.CreateTilePool();
        dividerPool.CreateDividerPool();
        flowerPool.CreateFlowerPool();
        gl.SpawnFirstGarden();
    }

    void Update()
    {

        if (gameState == GameState.StartScreen)
        {
            if (!playingTimeline && Input.GetKeyDown(KeyCode.B))
            {
                startTimeline.Play();
            }
        }
        else if (gameState == GameState.InGame)
        {
            CheckBoundaries();
        }
        else if (gameState == GameState.EndScreen)
        {

        }
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
            }
            else if (pc.transform.position.y < boundaryBottom)
            {
                // is too far down
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnBottom());
                pc.up = true;
                pc.down = false;
            }

            if (pc.transform.position.x < boundaryLeft)
            {
                // is too far left
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnLeft());
                pc.left = false;
                pc.right = true;
            }
            else if (pc.transform.position.x > boundaryRight)
            {
                // is too far right
                isOutOfBounds = true;
                pc.StartCoroutine(pc.OutOfBoundsReturnRight());
                pc.left = true;
                pc.right = false;
            }
        }
    }
}
