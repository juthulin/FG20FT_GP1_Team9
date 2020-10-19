using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    
    Camera _cam;
    Transform _cachedTransform;
    Transform _parentTransform;
    Vector3 _targetPosition;
    bool _showOutOfBounds;
    bool _hasTarget;
    bool _isDelivery;
    
    [SerializeField] Transform OoBArrowLookAt;
    [SerializeField] Transform uiVisualTransform;
    [SerializeField] Transform deliveryWaypointArrow;
    [SerializeField, Range(0.1f, 5f)] float arrowDistance;
    [SerializeField] GameObject waypointArrowVisual;
    [SerializeField] GameObject outOfBoundsArrowVisual;

    public static WaypointManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        _cachedTransform = transform;
        _parentTransform = transform.parent;
        waypointArrowVisual.transform.localPosition = Vector3.forward * arrowDistance;
        outOfBoundsArrowVisual.transform.localPosition = Vector3.forward * arrowDistance;
        outOfBoundsArrowVisual.gameObject.SetActive(false);
        _cam = Camera.main;
    }

    void Update()
    {
        if (_showOutOfBounds)
        {
            _cachedTransform.LookAt(OoBArrowLookAt.position);
            return;
        }
        
        if (!_hasTarget)
        {
            waypointArrowVisual.SetActive(false);
            uiVisualTransform.gameObject.SetActive(false);
            return;
        }

        deliveryWaypointArrow.localEulerAngles = _isDelivery ? Vector3.zero : new Vector3(0f, 0f, 180f);
        
        uiVisualTransform.position = _cam.WorldToScreenPoint(_targetPosition);
        
        Vector3 dirToTarget = (_targetPosition - _cachedTransform.position).normalized;
        float dotProduct = Vector3.Dot(_parentTransform.forward, dirToTarget);

        bool showWaypointArrow = dotProduct < 0f;
        
        waypointArrowVisual.SetActive(showWaypointArrow);
        uiVisualTransform.gameObject.SetActive(!showWaypointArrow);

        if (!showWaypointArrow) return;
        
        Vector3 lookAtPosition = new Vector3(_targetPosition.x, _cachedTransform.position.y, _targetPosition.z);
        _cachedTransform.LookAt(lookAtPosition);
    }

    public void SetTargetPosition(in Vector3 targetPositionWorldSpace,in bool isDelivery)
    {
        _hasTarget = true;
        _targetPosition = targetPositionWorldSpace;
        _isDelivery = isDelivery;
    }

    public void ClearTarget()
    {
        _hasTarget = false;
    }

    public void PlayerOutOfBounds(in bool isOutOfBounds)
    {
        _showOutOfBounds = isOutOfBounds;
        outOfBoundsArrowVisual.gameObject.SetActive(isOutOfBounds);
        waypointArrowVisual.SetActive(false);
    }

    void OnValidate()
    {
        waypointArrowVisual.transform.localPosition = Vector3.forward * arrowDistance;
        outOfBoundsArrowVisual.transform.localPosition = Vector3.forward * arrowDistance;
    }
}
