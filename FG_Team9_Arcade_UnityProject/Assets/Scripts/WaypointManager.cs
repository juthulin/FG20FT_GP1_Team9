using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    
    private Camera _cam;
    private Transform _cachedTransform;
    private Transform _parentTransform;
    private Vector3 _targetPosition;
    private bool _showOutOfBounds;
    private bool _hasTarget;
    private bool _isDelivery;
    
    [SerializeField] private Transform OoBArrowLookAt;
    [SerializeField] private Transform uiVisualTransform;
    [SerializeField] private Transform deliveryWaypointArrow;
    [SerializeField, Range(0.1f, 5f)] private float arrowDistance;
    [SerializeField] private GameObject waypointArrowVisual;
    [SerializeField] private GameObject outOfBoundsArrowVisual;

    public static WaypointManager Instance;
    private void Awake()
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

    private void Update()
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

    private void OnValidate()
    {
        waypointArrowVisual.transform.localPosition = Vector3.forward * arrowDistance;
        outOfBoundsArrowVisual.transform.localPosition = Vector3.forward * arrowDistance;
    }
}
