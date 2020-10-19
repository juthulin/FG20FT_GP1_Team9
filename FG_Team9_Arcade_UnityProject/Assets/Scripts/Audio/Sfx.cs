using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sfx : MonoBehaviour
{
    [Tooltip("Generic Sfx")]
    public AudioClip[] sfx;
    public bool playRandomSfxOnAwake;
    public bool playRandomSfxOnEnable;

    [Tooltip("Will Play Random Sfx on collision")]
    public bool hasCollisionSfx;
    public bool hasCollisionSfxOnTrigger;

    [Tooltip("Collision Sfx")]
    public AudioClip[] onCollisionSfx;
    public AudioClip[] onGetShotSfx;
    public AudioClip[] onShootSfx;
    public AudioClip[] onDeathSfx;
    public AudioClip[] onImpactSfx;

    public AudioSource audioSource;
    public AudioSource audioSourceCollisionSfx;
    public AudioSource audioSourceImpactSfx;
    public AudioSource audioSourceDeathSfx;

    public string tagCollisionExlusion;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSourceCollisionSfx == null)
        {
            audioSourceCollisionSfx = audioSource;
        }
        if (audioSourceImpactSfx == null)
        {
            audioSourceImpactSfx = audioSource;
        }
        if (playRandomSfxOnAwake)
        {
            PlayRandomSfx();
        }
    }


    private void OnEnable()
    {
        if (playRandomSfxOnEnable)
        {
            PlayRandomSfx();
        }
    }

    private void PlayRandomSfx()
    {
        audioSource.PlayOneShot(sfx[Random.Range(0, sfx.Length)]);
    }


    public void PlayShootSoundRandomPitch(float randomPitchMin, float randomPitchMax)
    {
        audioSource.pitch = Random.Range(randomPitchMin, randomPitchMax);
        audioSource.PlayOneShot(onShootSfx[Random.Range(0, onShootSfx.Length)]);
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag(tagCollisionExlusion))
        //{
        //    return;
        //}

        if (hasCollisionSfxOnTrigger)
        {
            audioSourceCollisionSfx.PlayOneShot(onCollisionSfx[Random.Range(0, onCollisionSfx.Length)]);
        }
    }

    public void PlayRandomDeathSFX()
    {
        audioSourceDeathSfx.PlayOneShot(onDeathSfx[Random.Range(0, onDeathSfx.Length)]);
    }

    public void PlayRandomImpactHitsSFX()
    {
        audioSourceImpactSfx.PlayOneShot(onImpactSfx[Random.Range(0, onImpactSfx.Length)]);
    }


    private void OnCollisionEnter(Collision collision)
    {

        //if (collision.gameObject.CompareTag(tagCollisionExlusion))
        //{
        //    return;
        //}

        if (hasCollisionSfx)
        {
            audioSourceCollisionSfx.PlayOneShot(onCollisionSfx[Random.Range(0, onCollisionSfx.Length)]);
        }

        if (collision.gameObject.tag == "PlayerProjectile")
        {
            audioSource.PlayOneShot(onShootSfx[Random.Range(0, onCollisionSfx.Length)]);
        }
    }
}
