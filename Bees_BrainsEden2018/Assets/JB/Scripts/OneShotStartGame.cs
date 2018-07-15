using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotStartGame : MonoBehaviour
{
    [SerializeField] Animator playerAnim;
    void OnEnable()
    {
        GameManager.gm.gameState = GameState.InGame;
        playerAnim.enabled = false;
        GameManager.gm.playerCam.Priority = 10;
        GameManager.gm.startCam.Priority = 1;
    }

}
