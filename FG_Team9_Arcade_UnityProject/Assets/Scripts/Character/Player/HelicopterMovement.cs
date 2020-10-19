using UnityEngine;

public class HelicopterMovement : PlayerMovement
{
	[Header("Gravity")] 
	public float gravityScale = 1f;

	[Header("Gas / Forward thrusters (local z-axis)")]
	public float forwardAcceleration = 2f;
	public float forwardDeAcceleration = 1f;
	public float maxForwardSpeed = 10f;
	
	
	[Header("Mouse")]
	public float mouseTurnSpeed = 70f;
	[Header("Pitch limits")]
	[Range(0f, 1f)] public float upperCameraPitchLimit = 0.7f;
	[Range(0f, 1f)] public float lowerCameraPitchLimit = 0.7f;

	
	[Header("Hover thrusters / vertical thrusters (local y-axis)")]
	public float upwardsAcceleration = 2f;
	public float upwardsDeacceleration = 1f;
	public float maxUpwardsSpeed = 10f;
	[Space]
	public float downwardsAcceleration = 2f;
	public float downwardsDeacceleration = 1f;
	public float maxDownwardsSpeed = 10f;

	[Header("Horizontal thrusters (local x-axis)")]
	public float strafeAcceleration = 2f;
	public float strafeDeacceleration = 1f;
	public float maxStrafeSpeed = 10f;

	[Header("Tilt")]
	public float pitchMultiplier = 1f;
	public float rollMultiplier = 1f;


	[Header("Override deacceleration speed when swapping from plane mode")]
	public float swapTimer = 2f;
	public float forwardDeAccelerationOverride = 1f;
	public float upwardsDeaccelerationOverride = 1f;
	public float downwardsDeaccelerationOverride = 1f;
	public float strafeDeaccelerationOverride = 1f;
	
	bool _swapOverride = false;
	
	float _timeSinceMovementModeSwap = 0f;
	bool _rotationSmoothingOverride = false;
	
	
	[Header("Rotation smoothing")]
	public float rollSmoothSpeed = 100f;
	public float pitchSmoothSpeed = 100f;
	[Tooltip("The duration the override will last after swapping movement mode")] 
	public float rotationSmoothingOverrideTimer = 3f;
	public float rollSmoothSpeedOverdrive = 25f;
	public float pitchSmoothSpeedOverdrive = 25f;

	public void ResetSwapTimer1()
	{	
		_timeSinceMovementModeSwap = 0f;
		_swapOverride = true;
		_rotationSmoothingOverride = true;
	}

	void Update()
	{
		_timeSinceMovementModeSwap += Time.deltaTime;
		if (_timeSinceMovementModeSwap > swapTimer)
		{
			_swapOverride = false;
		}

		if (_timeSinceMovementModeSwap > rotationSmoothingOverrideTimer)
		{
			_rotationSmoothingOverride = false;
		}
	}
	
	public void MoveShip(float xInput, float yInput, float zInput)
	{
		Vector3 forwardDir = _cam.transform.forward;
		Vector3 upDir = Vector3.up;
		Vector3 rightDir = _cam.transform.right;
		
		// forward
		if (zInput != 0)
		{
			CurrentForwardSpeed = Mathf.MoveTowards(CurrentForwardSpeed, maxForwardSpeed * zInput,
				forwardAcceleration * Time.fixedDeltaTime);
		}
		else if (_swapOverride)
		{
			CurrentForwardSpeed =
				Mathf.MoveTowards(CurrentForwardSpeed, 0f, forwardDeAccelerationOverride * Time.fixedDeltaTime);
		}
		else
		{
			CurrentForwardSpeed =
				Mathf.MoveTowards(CurrentForwardSpeed, 0f, forwardDeAcceleration * Time.fixedDeltaTime);
		}
		
		Vector3 forwardVelocity = forwardDir * CurrentForwardSpeed;

		// up
		if (yInput > 0f)
		{
			CurrentUpWardsSpeed = Mathf.MoveTowards(CurrentUpWardsSpeed, maxUpwardsSpeed,
				upwardsAcceleration * Time.fixedDeltaTime);
		}
		else if (_swapOverride)
		{
			CurrentUpWardsSpeed =
				Mathf.MoveTowards(CurrentUpWardsSpeed, 0f, upwardsDeaccelerationOverride * Time.fixedDeltaTime);
		}
		else
		{
			CurrentUpWardsSpeed =
				Mathf.MoveTowards(CurrentUpWardsSpeed, 0f, upwardsDeacceleration * Time.fixedDeltaTime);
		}

		Vector3 upwardVelocity = upDir * CurrentUpWardsSpeed;

		// down
		if (yInput < 0f)
		{
			CurrentDownWardsSpeed = Mathf.MoveTowards(CurrentDownWardsSpeed, maxDownwardsSpeed,
				downwardsAcceleration * Time.fixedDeltaTime);
		}
		else if (_swapOverride)
		{
			CurrentDownWardsSpeed =
				Mathf.MoveTowards(CurrentDownWardsSpeed, 0f, downwardsDeaccelerationOverride * Time.fixedDeltaTime);
		}
		else
		{
			CurrentDownWardsSpeed =
				Mathf.MoveTowards(CurrentDownWardsSpeed, 0f, downwardsDeacceleration * Time.fixedDeltaTime);
		}

		Vector3 downWardVelocity = -upDir * CurrentDownWardsSpeed;


		if (xInput != 0)
		{
			CurrentRightSpeed = Mathf.MoveTowards(CurrentRightSpeed, maxStrafeSpeed * xInput,
				strafeAcceleration * Time.deltaTime);
		}
		else if (_swapOverride)
		{
			CurrentRightSpeed = Mathf.MoveTowards(CurrentRightSpeed, 0f, strafeDeaccelerationOverride * Time.deltaTime);
		}
		else
		{
			CurrentRightSpeed = Mathf.MoveTowards(CurrentRightSpeed, 0f, strafeDeacceleration * Time.deltaTime);
		}
		
		Vector3 strafeVelocity = rightDir * CurrentRightSpeed;

		Vector3 totalVelocity = forwardVelocity + upwardVelocity + downWardVelocity +
		                        strafeVelocity
		                        + gravityScale * Vector3.down;
		
		_rigidbody.velocity = totalVelocity;
		
		Debug.DrawLine(transform.position, transform.position + totalVelocity);
		
	}

	
	public void Rotate()
	{
		if (!Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50f,
			mouseControlMask)) return;

		Vector3 thisForward = transformToYaw.forward;
		Vector3 hitPoint = hit.point;

		// rotate mouse transform to look at the hitpoint
		targetPivot.LookAt(hitPoint);

		// cross product between player forward (ignoring pitch) and mouse target direction
		Vector3 directionToMousePosTarget = targetPivot.forward;
		Vector3 cross = Vector3.Cross(thisForward, directionToMousePosTarget);
		cross = transformToYaw.InverseTransformVector(cross);

		// // yaw
		Vector3 yawAmount = new Vector3(0f, cross.y * mouseTurnSpeed * Time.deltaTime, 0f);
		transformToYaw.Rotate(yawAmount, Space.Self);

		//transformToPitch.rotation = transformToYaw.rotation;
		
		
		// roll mesh
		Quaternion bodyRoll = Quaternion.AngleAxis(-CurrentRightSpeed * rollMultiplier, transformToYaw.forward);
		Quaternion targetRotationRoll = bodyRoll * transformToYaw.rotation * _originalBodyRotation;

		
		// pitch
		Quaternion pitch = Quaternion.AngleAxis(CurrentForwardSpeed * pitchMultiplier, transformToYaw.right);

		Quaternion targetRotationPitch = pitch * transformToYaw.rotation;
		
		
		if (_rotationSmoothingOverride)
		{
			rollSmoothSpeed = rollSmoothSpeedOverdrive;
			pitchSmoothSpeed = pitchSmoothSpeedOverdrive;
		}

		bodyTransform.rotation = Quaternion.RotateTowards(bodyTransform.rotation, targetRotationRoll, rollSmoothSpeed * Time.fixedDeltaTime);
		transformToPitch.rotation =  Quaternion.RotateTowards(transformToPitch.rotation, targetRotationPitch, pitchSmoothSpeed * Time.fixedDeltaTime);
		
	}
	
	
}