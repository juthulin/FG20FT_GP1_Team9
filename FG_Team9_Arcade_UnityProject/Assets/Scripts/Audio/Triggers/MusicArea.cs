using UnityEngine;

public class MusicArea : MonoBehaviour
{
    public AudioClip musicToPlay;

    public bool isFadeWhenExitArea;

    public float musicFadeInDuration = 2f;
    public float musicFadeInVolume = 1f;

    public float currentmusicTime;
}
