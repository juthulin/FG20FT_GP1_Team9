using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[Header("General camera follow settings")]
	public float followPlayerSmoothTime = 0.5f;
	public float planeCameraTargetPlayerZdirOffset = 100f;
	public float hoverCameraTargetPlayerZdirOffset = 100f;

	[Header("FOV and FOV Smoothing")]
	[Range(0f, 100f)] public float fovAtMaxSpeed = 80f;
	public float changeToHoverFOVSmoothTime = 2f;
	public float changeToPlaneFOVSmoothTime = 2f;
	
	[Header("Setup")] 
	public Player playerRef;
	public HelicopterMovement helicoptermovement;
	public Transform playerPitchTransform;
	public Transform targetPivotTransform;
	
	
	
	Vector3 _currentLookAt;
	Vector3 _targetLookAtPos = Vector3.zero;

	Vector3 _currentCameraVelocity;
	
	float _originalFOV;
	float _fovVelocity;
	
	Camera _cam;
	
	void Awake()
	{
		_currentLookAt = playerPitchTransform.position + playerPitchTransform.forward;

		_cam = Camera.main;

		_originalFOV = _cam.fieldOfView;
	}

	void FixedUpdate()
	{
		transform.position = playerPitchTransform.position;
		
		if (playerRef.usePlaneMovement)
		{
			_targetLookAtPos = playerPitchTransform.position + playerPitchTransform.forward * planeCameraTargetPlayerZdirOffset;
			
			float minForwardSpeed = playerRef._movement.MinForwardSpeed();
			float maxForwardSpeed = playerRef._movement.MaxForwardSpeed();
			
			float t = Mathf.InverseLerp(minForwardSpeed, maxForwardSpeed,  PlayerMovement.CurrentForwardSpeed);
			float targetFOV = Mathf.Lerp(_originalFOV, fovAtMaxSpeed, t);

			if (Mathf.Approximately(_cam.fieldOfView, targetFOV))
			{
				_cam.fieldOfView = targetFOV;
			}
			else
			{
				// smooth into new fov when changing from hover mode
				_cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, targetFOV, ref _fovVelocity,
					changeToPlaneFOVSmoothTime);
			}
		}
		else
		{
			float cameraPitch = Vector3.Dot(Vector3.up, targetPivotTransform.forward);
			
			if (cameraPitch < helicoptermovement.upperCameraPitchLimit && cameraPitch > -helicoptermovement.lowerCameraPitchLimit)
			{
				_targetLookAtPos = targetPivotTransform.position + targetPivotTransform.forward * hoverCameraTargetPlayerZdirOffset;
			}

			if (!Mathf.Approximately(_cam.fieldOfView, _originalFOV))
			{
				// smooth into new fov when changing from plane mode
				_cam.fieldOfView = Mathf.SmoothDamp(_cam.fieldOfView, _originalFOV, ref _fovVelocity, changeToHoverFOVSmoothTime);
			}
		}

		_currentLookAt = Vector3.SmoothDamp(_currentLookAt, _targetLookAtPos, ref _currentCameraVelocity, followPlayerSmoothTime);
		
		transform.LookAt(_currentLookAt);
	}
}