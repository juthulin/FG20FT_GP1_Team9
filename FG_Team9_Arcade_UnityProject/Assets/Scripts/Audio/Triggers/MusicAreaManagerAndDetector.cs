using UnityEngine;

public class MusicAreaManagerAndDetector : MonoBehaviour
{
    public MusicArea currentMusicArea;
    public MusicManager musicManager;

    void Awake()
    {
        musicManager = FindObjectOfType<MusicManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MusicArea>() != null)
        {
            currentMusicArea = other.GetComponent<MusicArea>();
            musicManager.PlayCrossFadeMusic(currentMusicArea.musicToPlay, currentMusicArea.musicFadeInDuration, currentMusicArea.musicFadeInVolume, currentMusicArea.currentmusicTime);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentMusicArea != null)
        {
            if(musicManager.currentAudioTrack == musicManager.audioTrack1)
            {
                currentMusicArea.currentmusicTime = musicManager.audioTrack1.time + currentMusicArea.musicFadeInDuration;
            }
            else
            {
                currentMusicArea.currentmusicTime = musicManager.audioTrack2.time + currentMusicArea.musicFadeInDuration;
            }
            if (currentMusicArea.isFadeWhenExitArea)
            {
                musicManager.FadeOutCurrentAudioTrack(currentMusicArea.musicFadeInDuration, 0f);
            }
        }   
    }
}
