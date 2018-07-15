using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    public Image fader;
    public Color fadeInStartColour;
    Color chosenFadeColour;
    public float fadeLength = 1;
    public float scoreValue;
    public Text scoreText;

    public RectTransform endScreen;
    public Text scoreTextEndScreen;

    public AudioSource mainMusic;

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
        FadeIn(fadeInStartColour);
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

    public void OnDeath()
    {
        mainMusic.Stop();
        gameState = GameState.EndScreen;
        pc.anim.SetTrigger("onDeath");
        pc.ResetVariables();
        FadeOut();
        Invoke("ShowEndScreen", fadeLength * 1.2f);
        scoreText.gameObject.SetActive(false);
        dividerPool.RecallDividers();
        flowerPool.RecallDividers();
        tilePool.RecallTiles();
    }

    public void ShowEndScreen()
    {
        endScreen.gameObject.SetActive(true);
        scoreTextEndScreen.text = scoreValue.ToString();
    }

    void ResetLevel()
    {
        gameState = GameState.StartScreen;
        pc.anim.SetTrigger("onReset");
        endScreen.gameObject.SetActive(false);
        startCam.Priority = 10;
        playerCam.Priority = 1;
        scoreValue = 0;
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

    #region Fading

    public void FadeIn()
    {
        chosenFadeColour = Color.black;
        StartCoroutine(FadeInCoroutine());
    }

    public void FadeIn(Color _col)
    {
        chosenFadeColour = _col;
        StartCoroutine(FadeInCoroutine());
    }


    IEnumerator FadeInCoroutine()
    {
        float progress = 0;
        float timer = 0;

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / fadeLength;
            fader.color = Color.Lerp(chosenFadeColour, Color.clear, progress);

            yield return null;
        }
    }

    public void FadeOut()
    {
        chosenFadeColour = Color.black;
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeOut(Color _col)
    {
        chosenFadeColour = _col;
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        float progress = 0;
        float timer = 0;

        while (progress < 1)
        {
            timer += Time.deltaTime;
            progress = timer / fadeLength;
            fader.color = Color.Lerp(Color.clear, chosenFadeColour, progress);

            yield return null;
        }
    }

    #endregion
}
