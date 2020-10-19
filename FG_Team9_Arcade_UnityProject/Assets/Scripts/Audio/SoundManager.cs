using UnityEngine;

public static class SoundManager
{
    public static void PlaySound(AudioClip clipToPlay, Vector3 positionOfSound, float clipVolume = 1.0f, float maxDistance = 100f)
    {

        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        
        audioSource.clip = clipToPlay;
        audioSource.transform.position = positionOfSound;
        audioSource.volume = clipVolume;
        audioSource.maxDistance = maxDistance;

        audioSource.Play();

        Object.Destroy(soundGameObject, audioSource.clip.length);

    }
}
