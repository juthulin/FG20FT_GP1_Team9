using UnityEngine;

public class PlaySFXOnCollision : MonoBehaviour
{

    public AudioClip clipToPlay;
    public float volume = 0.1f;

    private Transform _transform;
    private void Awake()
    {
        _transform = transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        SoundManager.PlaySound(clipToPlay, _transform.position, volume);
    }
}
