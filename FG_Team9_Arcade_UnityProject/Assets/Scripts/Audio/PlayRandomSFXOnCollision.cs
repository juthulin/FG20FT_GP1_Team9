using UnityEngine;

public class PlayRandomSFXOnCollision : MonoBehaviour
{

    public AudioClip[] audioClips;
    public float volume = 0.1f;

    private Transform _transform;
    private void Awake()
    {
        _transform = transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioClip _clipToPlay = audioClips[Random.Range(0, audioClips.Length)];
        SoundManager.PlaySound(_clipToPlay, _transform.position, volume);
    }
}
