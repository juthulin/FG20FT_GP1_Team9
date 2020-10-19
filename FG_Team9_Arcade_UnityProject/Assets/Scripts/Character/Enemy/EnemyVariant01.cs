using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Idle,
    Pursuit,
    Flee,
    Disabled
}

public class EnemyVariant01 : MonoBehaviour
{
    //------
    [Header("Temporary Variables")]
    Color _gizmoColor;
    //------
    
    Rigidbody _rigidbody;
    EnemySpawner _enemySpawner;
    Transform _cachedTransform;
    Transform _cachedPlayerTf;
    Vector3 _targetPos;
    Vector3 _waypoint = default;
    Vector3 _velocity = default;
    Vector3 _position = default;
    Vector3 _forward = default;
    float _fleeTimer = default;
    float _dragAtAwake = default;
    bool _canFire = true;
    bool _hasAcquiredTarget = false;
    bool _chaseTarget = false;
    EnemyState enemyState;
    
    [SerializeField] BaseEnemySetting enemySetting;
    [Space] 
    [SerializeField] GameObject disabledParticleEffect;

    void Awake()
    {
        _cachedTransform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _enemySpawner = EnemySpawner.instance;
    
        float startSpeed = (enemySetting.minSpeed + enemySetting.maxSpeed) / 2;
        _velocity = transform.forward * startSpeed;
        _dragAtAwake = _rigidbody.drag;
    }

    void OnEnable()
    {
        enemyState = _enemySpawner.PlayerHasCargo ? EnemyState.Pursuit : EnemyState.Idle; StartCoroutine(WaypointUpdate(3f, 5f));
        _position = _cachedTransform.position;
        _forward = _cachedTransform.forward;
        _rigidbody.useGravity = false;
        _rigidbody.drag = _dragAtAwake;
        disabledParticleEffect.SetActive(false);
    }

    void Update() //todo put waypoint into acceleration and let the only target be player, check with bool if should follow target.
    {
        if (enemyState == EnemyState.Disabled) return;
        
        Vector3 acceleration = Vector3.zero;
        
        Action();

        if (_chaseTarget)
        {
            Vector3 offsetToTarget = (_cachedPlayerTf.position - _position);
            acceleration = SteerTowards(offsetToTarget) * enemySetting.targetWeight;
        }
        else
        {
            Vector3 wayPointForce = SteerTowards(_waypoint) * enemySetting.wayPointWeight;
            acceleration += wayPointForce;
        }

        if (IsHeadingForCollision())
        {
            _rigidbody.drag = Mathf.Lerp(_rigidbody.drag, enemySetting.maxDrag, Time.deltaTime * enemySetting.dragLerpSpeed);
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * enemySetting.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }
        else if (_rigidbody.drag > enemySetting.normalDrag)
        {
            _rigidbody.drag = Mathf.Lerp(_rigidbody.drag, enemySetting.normalDrag, Time.deltaTime * (enemySetting.dragLerpSpeed * 0.5f));
        }

        _velocity += acceleration * Time.deltaTime;
        float speed = _velocity.magnitude;
        Vector3 dir = _velocity / speed;
        speed = Mathf.Clamp(speed, enemySetting.minSpeed, enemySetting.maxSpeed);
        _velocity = dir * speed;
        
        _rigidbody.AddForce(_velocity * (Time.deltaTime * enemySetting.forceModifier), ForceMode.VelocityChange);
        _cachedTransform.forward = Vector3.Slerp(_cachedTransform.forward, dir, Time.deltaTime * enemySetting.rotSpeed);
        _position = _cachedTransform.position;
        _forward = dir;
    }

    void OnDisable()
    {
        _enemySpawner.SpawnedEnemies.Remove(this.transform);
    }

    void Action()
    {
        Vector3 distanceToPlayer = _cachedPlayerTf.position - _position;
        float floatDistanceToPlayer = distanceToPlayer.magnitude;

        switch (enemyState)
        {
            case EnemyState.Idle: // IDLE state -----------------------
                if (/*floatDistanceToPlayer < enemySetting.detectionRadius || */_enemySpawner.PlayerHasCargo) SetPursuitState();
                break;
            
            case EnemyState.Pursuit: // PURSUIT state -----------------
                if(!_enemySpawner.PlayerHasCargo)
                {
                    SetFleeState();
                    break;
                }
                Vector3 directionToPlayer = distanceToPlayer.normalized;

                float facingPlayerPreciseness = Vector3.Dot(_forward, directionToPlayer);
                Ray ray = new Ray(_cachedTransform.position, directionToPlayer);
                
                if ( facingPlayerPreciseness > enemySetting.shootAtPreciseness && !Physics.Raycast(ray, floatDistanceToPlayer - 3))
                {
                    if (_canFire )
                    {
                        _canFire = false;
                        StartCoroutine(ShootAtPlayer());
                    }
                }
                else
                {
                    if (!_canFire)
                    {
                        _canFire = true;
                        StopAllCoroutines();
                    }
                }

                _hasAcquiredTarget = (facingPlayerPreciseness > 0f && !_hasAcquiredTarget);

                if (facingPlayerPreciseness < 0f || floatDistanceToPlayer < enemySetting.tooCloseThreshold)
                {
                    StopAllCoroutines();
                    if (_hasAcquiredTarget) SetFleeState();
                    _hasAcquiredTarget = false;
                }
                
                break;
            
            case EnemyState.Flee: // FLEE state -----------------------

                if (_fleeTimer <= 0f)
                {
                    StopAllCoroutines();
                    if (_enemySpawner.PlayerHasCargo) SetPursuitState();
                    else SetIdleState();
                }

                _fleeTimer -= Time.deltaTime;
                break;
        }
    }

    public void Initialize(Transform player)
    {
        _cachedPlayerTf = player;
    }
    
    Vector3 SteerTowards(Vector3 direction)
    {
        Vector3 v = direction.normalized * enemySetting.maxSpeed - _velocity;
        return Vector3.ClampMagnitude(v, enemySetting.maxSteerForce);
    }

    #region SetWaypoint
    void SetWaypoint()
    {
        Vector3 forward = _cachedTransform.forward;
        Vector3 up = _cachedTransform.up;
        Vector3 right = _cachedTransform.right;
        Vector3 position = _cachedTransform.position;

        float diff = 100f;
        float rndX = Random.Range(-diff, diff);
        float rndY = Random.Range(-diff, diff);
        float rndZ = Random.Range(diff / 2, diff * 3);
        
        float x = position.x + (right.x * rndX);
        float y = position.y + (up.y * rndY);
        float z = position.z + (forward.z * rndZ);
        
        _waypoint = (new Vector3(x, y, z) - position).normalized;
    }

    void SetWaypoint(Vector3 targetPosition)
    {
        float diff = 20f;
        float rndX = Random.Range(-diff, diff);
        float rndY = Random.Range(-diff, diff);
        float rndZ = Random.Range(-diff, diff);
        
        float x = targetPosition.x + rndX;
        float y = targetPosition.y + rndY;
        float z = targetPosition.z + rndZ;
        
        _waypoint = (new Vector3(x, y, z) - _cachedTransform.position).normalized;
    }
    #endregion

    void Shoot()
    {
        float a = enemySetting.shotInaccuracy;
        Vector2 shotDir = Vector2.zero;
        if (a != 0)
        {
            float rndX = Random.Range(-a, a);
            float rndY = Random.Range(-a, a);

            shotDir = new Vector2(_forward.x + rndX, _forward.y + rndY);
        }

        GameObject projectile = ProjectilePool.Instance.GetPooledObject(enemySetting.projectileType);
        projectile.transform.position = _cachedTransform.position + _cachedTransform.forward * 2;
        projectile.transform.rotation = _cachedTransform.rotation * Quaternion.Euler(shotDir);
        projectile.SetActive(true);
    }

    void SetIdleState()
    {
        enemyState = EnemyState.Idle;
        _chaseTarget = false;
        
        StartCoroutine(WaypointUpdate(3f, 5f));
    }

    void SetPursuitState()
    {
        enemyState = EnemyState.Pursuit;
        _chaseTarget = true;
    }
    
    void SetFleeState()
    {
        enemyState = EnemyState.Flee;
        _chaseTarget = false;
        
        _fleeTimer = enemySetting.fleeTimer;
        StartCoroutine(WaypointUpdate(0.3f, 1.2f));
    }
    
    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(_position, enemySetting.boundsRadius, _forward,
            out hit, enemySetting.collisionAvoidDist, enemySetting.obstacleMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = EnemyHelper.Directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = _cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(_position, dir);
             if (!Physics.SphereCast(ray, enemySetting.boundsRadius, enemySetting.collisionAvoidDist, enemySetting.obstacleMask))
             {
                 return dir;
             }
        }
        return _forward;
    }

    public void DisableEnemy()
    {
        enemyState = EnemyState.Disabled;
        _rigidbody.useGravity = true;
        _rigidbody.drag = 0f;
        disabledParticleEffect.SetActive(true);
        StartCoroutine(StartDeathTimer());
    }

    IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(.2f);
        }
    }
    
    IEnumerator WaypointUpdate(float minRndCooldown, float maxRndCooldown)
    {
        while(enemyState != EnemyState.Pursuit)
        {
            if (enemyState == EnemyState.Idle) SetWaypoint(_enemySpawner.ConvergePoint);
            else SetWaypoint();

            _targetPos = _waypoint;
            
            float rndCooldown = Random.Range(minRndCooldown, maxRndCooldown);
            yield return new WaitForSeconds(rndCooldown);
        }
    }

    IEnumerator StartDeathTimer()
    {
        yield return new WaitForSeconds(enemySetting.timeDisabled);
        gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.cyan;
        // Gizmos.DrawWireSphere(transform.position, enemySetting.detectionRadius);
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, enemySetting.tooCloseThreshold);
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawSphere(_waypoint, 1f);
        //
        // Gizmos.color = _gizmoColor;
        // Gizmos.DrawRay(transform.position, transform.forward * (enemySetting.collisionAvoidDist + enemySetting.boundsRadius));
        if (enemyState == EnemyState.Idle)
        {
            Gizmos.color = Color.green;
        }
        else if (enemyState == EnemyState.Pursuit)
        {
            Gizmos.color = Color.red;
        }
        else if (enemyState == EnemyState.Flee)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.white;
        }

        Gizmos.DrawSphere(transform.position, 3f);
    }
}