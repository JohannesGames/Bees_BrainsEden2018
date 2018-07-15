using UnityEngine;
using System.Collections;

public class PlayingTimelineBool : MonoBehaviour
{
    public bool changeTo;

    void OnEnable()
    {
        GameManager.gm.playingTimeline = changeTo;
    }
}
