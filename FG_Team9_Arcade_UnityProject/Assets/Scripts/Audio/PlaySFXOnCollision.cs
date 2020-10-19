using UnityEngine;

public class PlaySFXOnCollision : MonoBehaviour
{

    public AudioClip clipToPlay;
    public float volume = 0.1f;

    Transform _transform;
    void Awake()
    {
        _transform = transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        SoundManager.PlaySound(clipToPlay, _transform.position, volume);
    }
}
