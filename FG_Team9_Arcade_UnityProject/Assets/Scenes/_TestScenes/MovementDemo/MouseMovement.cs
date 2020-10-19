using UnityEngine;

public class MouseMovement : PlayerMovement
{
	[Header("Gravity")] public float gravityScale = 0f;

	[Header("Gas / Forward thrusters (local z-axis)")]
	public float forwardAcceleration = 10f;
	public float forwardDeacceleration = 15f;
	[Space] public float minForwardSpeed = 10f;
	public float maxForwardSpeed = 35f;
	
	[Header("Mouse control settings")]
	[Space]
	public float keyTurnSpeed = 70f;
	public float mouseTurnSpeed = 70f;
	public float mousePitchSpeed = 70f;
	[Range(0, 1)] public float shipPitchUpLimiter = 0.7f;
	[Range(0, 1)] public float shipPitchDownLimiter = 0.7f;

	[Header("Strafe speed")] public float maxStrafeSpeed = 5f;
	public float strafeAcceleration = 2f;
	public float strafeDeAcceleration = 1f;
	
	
	[Header("Deacceleration speed when swapping from helicopter mode")]
	public float upwardsDeacceleration = 1f;
	public float downwardsDeacceleration = 1f;
	
	[Header("Timed override")]
	public float forwardDeaccelerationOverride = 1f;
	public float swapTimerDeacceleration = 2f;

	
	float _lastPitchAngle = 0f;
	const float PitchMultiplier = 50f; // magic number that makes the pitch speed property have more similar effect to the yaw speed property
	
	
	bool _swapOverride = false;
	float _timeSinceMovementModeSwap = 0f;

	
	public void ResetSwapTimer1()
	{	
		_timeSinceMovementModeSwap = 0f;
		_swapOverride = true;
	}

	void Update()
	{
		_timeSinceMovementModeSwap += Time.deltaTime;
		if (_timeSinceMovementModeSwap > swapTimerDeacceleration)
		{
			_swapOverride = false;
		}
	}
	
	public void MoveShip(float xInput, float yInput, float zInput)
	{
		Vector3 forwardDir = _cam.transform.forward;
		Vector3 upDir = Vector3.up;
		Vector3 rightDir = _cam.transform.right;

		if (zInput > 0f)
		{
			CurrentForwardSpeed =
				Mathf.MoveTowards(CurrentForwardSpeed, maxForwardSpeed, forwardAcceleration * Time.fixedDeltaTime);
		}
		else if (_swapOverride)
		{
			CurrentForwardSpeed =
				Mathf.MoveTowards(CurrentForwardSpeed, minForwardSpeed, forwardDeaccelerationOverride * Time.fixedDeltaTime);
		}
		else
		{
			CurrentForwardSpeed =
				Mathf.MoveTowards(CurrentForwardSpeed, minForwardSpeed, forwardDeacceleration * Time.fixedDeltaTime);
		}

		Vector3 forwardVelocity = forwardDir * CurrentForwardSpeed;

		if (xInput != 0)
		{
			CurrentRightSpeed = Mathf.MoveTowards(CurrentRightSpeed, maxStrafeSpeed * xInput,
				strafeAcceleration * Time.deltaTime);
		}
		else
		{
			CurrentRightSpeed = Mathf.MoveTowards(CurrentRightSpeed, 0f, strafeDeAcceleration * Time.fixedDeltaTime);
		}
		
		Vector3 strafeVelocity = rightDir * CurrentRightSpeed;

		
		CurrentUpWardsSpeed = Mathf.MoveTowards(CurrentUpWardsSpeed, 0f, upwardsDeacceleration * Time.fixedDeltaTime);
		Vector3 upwardVelocity = upDir * CurrentUpWardsSpeed;
		CurrentDownWardsSpeed = Mathf.MoveTowards(CurrentDownWardsSpeed, 0f, downwardsDeacceleration * Time.fixedDeltaTime);
		Vector3 downWardVelocity = -upDir * CurrentDownWardsSpeed;


		Vector3 totalVelocity = forwardVelocity + upwardVelocity + downWardVelocity +
		                        strafeVelocity
		                        + gravityScale * Vector3.down;
		
		_rigidbody.velocity = totalVelocity;
	}

	
	public void Rotate(float xInput)
	{
		Vector3 yawAmount = Vector3.zero;

		if (xInput < 0f)
		{
			yawAmount = new Vector3(0f, -keyTurnSpeed * Time.deltaTime);
		}
		else if (xInput > 0f)
		{
			yawAmount = new Vector3(0f, keyTurnSpeed * Time.deltaTime);
		}


		if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 50f, mouseControlMask))
		{
			Vector3 thisForward = transformToYaw.forward;
		
			Vector3 hitPoint = hit.point;

			// rotate mouse transform to look at the hitpoint
			targetPivot.LookAt(hitPoint);

			// cross product between player forward (ignoring pitch) and mouse target direction
			Vector3 directionToMousePosTarget = targetPivot.forward;
			Vector3 cross = Vector3.Cross(thisForward, directionToMousePosTarget);
		
			cross = transformToYaw.InverseTransformVector(cross);

			// // yaw
			yawAmount += new Vector3(0f, cross.y * mouseTurnSpeed * Time.deltaTime, 0f);
			transformToYaw.Rotate(yawAmount, Space.Self);
		
			// pitch
			float closeNessToUp = Vector3.Dot(transformToPitch.forward, Vector3.up);
		
			float targetAngle = cross.x * mousePitchSpeed * PitchMultiplier;

			bool reachedUpperLimit = closeNessToUp > shipPitchUpLimiter;
			bool reachedLowerLimit = closeNessToUp < -shipPitchDownLimiter;

			bool intendsToGoUp = targetAngle <= _lastPitchAngle;
		
			bool cantGoUp = reachedUpperLimit && intendsToGoUp;
			bool cantGoDown = reachedLowerLimit && !intendsToGoUp;

			
			// reset any body rotation made in the other movement mode
			bodyTransform.localRotation = _originalBodyRotation;

			if (!(cantGoUp || cantGoDown))
			{
				Quaternion targetRotationPitch = Quaternion.AngleAxis(targetAngle * Time.deltaTime, transformToPitch.right) * transformToYaw.rotation;
				
				transformToPitch.rotation = targetRotationPitch;
				_lastPitchAngle = targetAngle;
			}
		}
		
	}
}