using System;
using UnityEngine;

[RequireComponent(typeof(HealthSystem), typeof(GattlingGun), typeof(EMP))]
[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [NonSerialized]
    public bool usePlaneMovement = true;
    public float collisionDamage = 2f;

    [Header("Setup")]
    public Transform fireFrom;
    public Transform target;
    public int enemyKillMultiplier;

    GattlingGun _gattlingGun;
    EMP _emp;
    
    public Texture2D mouseCursor;
    
    public PlayerMovement _movement;

    HealthSystem _healthSystem;
    
    [SerializeField] ParticleSystem _hoverLeft;
    [SerializeField] ParticleSystem _hoverRight;
    [SerializeField] ParticleSystem _engine;
    [SerializeField] ParticleSystem _magnet;
    
    void Awake()
    {
        
        _movement = GetComponent<PlayerMovement>();
        _healthSystem = GetComponent<HealthSystem>();

        // weapons
        _gattlingGun = GetComponent<GattlingGun>();
        _emp = GetComponent<EMP>();
        

        // mouse cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Vector2 cursorCenter = new Vector2(mouseCursor.width / 2f, mouseCursor.height / 2f);
        Cursor.SetCursor(mouseCursor, cursorCenter, CursorMode.Auto);
    }
    
    void Update()
    {
        GetInputVariables();

        _gattlingGun.fire = _fireInput;
        
        _magnet.gameObject.SetActive(PickupManager.Instance.playerIsHoldingPickup);

        // Debug
        Debug.DrawLine(fireFrom.position, fireFrom.position + fireFrom.forward * 1000f, Color.yellow);
    }

    void FixedUpdate()
    {
        _movement.MoveShip(_xAxisInput, _yAxisInput, _zAxisInput, usePlaneMovement);
    }
    
    void OnCollisionEnter(Collision other)
    {
        if (PickupManager.Instance.playerIsHoldingPickup)
        {
            Vector3 contactPoint = other.GetContact(0).point;
            Vector3 normal = other.GetContact(0).normal;
            Vector3 forwardDir = _movement.transformToPitch.forward;
            
            // draw normal
            Debug.DrawLine(contactPoint, contactPoint + -normal * 10f, Color.red, 10f);
            Debug.DrawLine(contactPoint, contactPoint + forwardDir * 10f, Color.yellow, 10f);
            
            
            // this isn't technically accurate, since all of the velocity isn't forward, but it's good enough, no need for the extra calculations
            // higher dot = crashing more straight into the thingey 
            float dot = Vector3.Dot(-normal, forwardDir * PlayerMovement.CurrentForwardSpeed);
            
            _healthSystem.Damage(collisionDamage * dot);
            GameObject ps = ParticlePool.Instance.GetPooledObject(ParticleType.PlayerBuildingCollisionSparks);
            ps.transform.position = other.GetContact(0).point;
            ps.SetActive(true);
        }
    }

    #region Input
    
    // directional axis input
    float _xAxisInput;
    float _yAxisInput;
    float _zAxisInput;
    
    //firing
    bool _fireInput = false;
    
    void GetInputVariables()
    {
        // directional axis input
        _xAxisInput = Input.GetAxisRaw("Horizontal");
        _yAxisInput = Input.GetAxisRaw("Vertical");
        _zAxisInput = Input.GetAxisRaw("Forwards");
        
        // weapons
        _fireInput = Input.GetMouseButton(0);
        _emp.isChargingEMP = Input.GetMouseButton(1);
        
        // Pickup drop
        if (Input.GetKey(KeyCode.Q))
        {
            PickupManager.Instance.OnDrop();
        }
        
        //swap Movement mode 
        if (Input.GetKeyDown(KeyCode.T))
        {
            usePlaneMovement = !usePlaneMovement;
            _movement._usingPlaneMovement = usePlaneMovement;
            
            _movement.ResetSwapTimer();
            
            
            CheckHoverFlightParticles();
        }
    }
    
    #endregion

    void CheckHoverFlightParticles()
    {
        _hoverLeft.gameObject.SetActive(!usePlaneMovement);
        _hoverRight.gameObject.SetActive(!usePlaneMovement);
        _engine.gameObject.SetActive(usePlaneMovement);
    }

    
}