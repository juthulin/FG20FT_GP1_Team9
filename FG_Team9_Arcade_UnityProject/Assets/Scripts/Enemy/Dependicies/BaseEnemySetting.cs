using UnityEngine;

[CreateAssetMenu(menuName = "Base Enemy Setting")]
public class BaseEnemySetting : ScriptableObject
{
    public float tooCloseThreshold;
    public float fleeTimer;
    [Range(0.5f, 1.0f)] public float shootAtPreciseness;
    public ProjectileType projectileType;
    public float detectionRadius;
    public LayerMask obstacleMask;

    public float minSpeed;
    public float maxSpeed;
    public float maxSteerForce;
    public float targetWeight;
    public float wayPointWeight;
    
    public float boundsRadius;
    public float collisionAvoidDist;
    public float avoidCollisionWeight;

    [Range(1, 1000)] public int forceModifier;
    public float rotSpeed;
    public float maxDrag;
    public float normalDrag;
    public float dragLerpSpeed;

    [Range(0, 20)] public int shotInaccuracy;
    public float timeDisabled;
}
