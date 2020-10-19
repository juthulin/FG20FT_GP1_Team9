using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour, IProjectile
{
    public float damageToApply = 2f;
    public float despawnTime = 3f;
    public float projectileSpeed = 90f;

    public float DamageToApply => damageToApply;

    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _rb.velocity = projectileSpeed * transform.forward;

        StartCoroutine(DelayedDeactivate());
    }

    IEnumerator DelayedDeactivate()
    {
        yield return new WaitForSeconds(despawnTime);
        Deactivate();
    }

    public void Deactivate()
    {
        _rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
