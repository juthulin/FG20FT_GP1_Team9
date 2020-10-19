using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public bool playOnStart = true;

    public AudioClip cityChillMusic;
    public AudioClip actionSuspenseMusic;
    public AudioClip successfulDeliveryMusic;
    public AudioClip successfulDeliveryMusicMaximum;
    public AudioClip failedDeliveryMusic;
    public AudioClip gameOverMusic;
    public AudioClip jazzMusic;

    public AudioSource audioTrack1;
    public AudioSource audioTrack2;

    public AudioClip actionStingerAmajor;
    public AudioClip actionStingerEmajor;
    public AudioClip actionStingerFmajor;

    public AudioSource importantAudioCues;

    public AudioClip clipToPlayOnStart;
    public AudioClip clipToFadeTo;

    public AudioSource currentAudioTrack;

    public AudioMixer mainAudioMixer;

    // Position of each music track

    private float _oldIdleMusicPos;
    private float _oldActionMusicPos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        currentAudioTrack = audioTrack1;
        if (playOnStart)
        {
            audioTrack1.clip = clipToPlayOnStart;
            audioTrack1.Play();
        }
    }

    public void PlayCrossFadeMusic(AudioClip clipToPlay, float duration = 1f, float targetVolume = 1f, float audioPosition = 0)
    {
        if(currentAudioTrack == audioTrack1)
        {
            StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic1", duration, 0f));
            StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic2", duration, targetVolume));
            currentAudioTrack = audioTrack2;
        }
        else
        {
            StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic2", duration, 0f));
            StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic1", duration, targetVolume));
            currentAudioTrack = audioTrack1;
        }
        currentAudioTrack.time = audioPosition;
        currentAudioTrack.clip = clipToPlay;
        currentAudioTrack.Play();
    }

    public void FadeOutCurrentAudioTrack(float duration = 1f, float targetVolume = 1f)
    {
        if (currentAudioTrack == audioTrack1)
        {
            StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic1", duration, 0f));
        }
        else
        {
            StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic2", duration, 0f));
        }
    }

    public void FadeOutAllMusic(float duration = 1.0f)
    {
        StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic1", duration, 0f));
        StartCoroutine(FadeMixerGroup.StartFade(mainAudioMixer, "VolumeMusic2", duration, 0f));
    }

    public void SetLoopForAllAudioTracks(bool activate)
    {
        audioTrack1.loop = activate;
        audioTrack2.loop = activate;
    }

    // Specific Music Events

    public void OnIdleBetweenDelivery()
    {
        if (currentAudioTrack.clip == actionSuspenseMusic)
        {
            _oldActionMusicPos = currentAudioTrack.time;
        }
        PlayCrossFadeMusic(cityChillMusic, 1,1,_oldIdleMusicPos);
    }

    public void OnPickup() // To Do Add Stinger hit fanfare in Important that always plays on this cue
    {
        if (currentAudioTrack.clip == cityChillMusic) 
        {
            _oldIdleMusicPos = currentAudioTrack.time;
        }
        AudioClip actionStingerToPlay = CheckActionStingerTime(_oldActionMusicPos);
        importantAudioCues.PlayOneShot(actionStingerToPlay);
        PlayCrossFadeMusic(actionSuspenseMusic,0.1f,1, _oldActionMusicPos);
    }

    public void OnSuccessfulDelivery()
    {
        importantAudioCues.PlayOneShot(successfulDeliveryMusic);
    }
    public void OnSuccessfulDeliveryMaximum()
    {
        importantAudioCues.PlayOneShot(successfulDeliveryMusicMaximum);
    }

    public void OnFailedPickup()
    {
        importantAudioCues.PlayOneShot(failedDeliveryMusic);
        
    }

    public void GameOverMusic()
    {
        importantAudioCues.PlayOneShot(gameOverMusic);
        currentAudioTrack.Stop();
    }


    private AudioClip CheckActionStingerTime(float actionClipTime)
    {
        AudioClip thisStingerToPlay;
        if(actionClipTime < 0.10f) // Worse, but probably best key change check ever :DDDDD
        {
            thisStingerToPlay = actionStingerAmajor;
        }
        else if (actionClipTime < 0.25f)
        {
            thisStingerToPlay = actionStingerEmajor;
        }
        else if(actionClipTime < 1.40f)
        {
            thisStingerToPlay = actionStingerAmajor;
        }
        else
        {
            thisStingerToPlay = actionStingerFmajor;
        }

        return thisStingerToPlay;
    }

}


