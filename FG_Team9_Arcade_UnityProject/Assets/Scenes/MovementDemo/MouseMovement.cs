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
	
	[Tooltip("The duration the override will last after swapping movement mode")] 
	public float rotationSmoothingOverrideTimer = 3f;
	
	[Header("Timed override")]
	public float forwardDeaccelerationOverride = 1f;
	public float swapTimerDeacceleration = 2f;

	
	float _lastPitchAngle = 0f;
	const float PitchMultiplier = 50f; // magic number that makes the pitch speed property have more similar effect to the yaw speed property
	
	
	bool _swapOverride = false;
	float _timeSinceMovementModeSwap = 0f;
	bool _rotationSmoothingOverride = false;

	
	public void ResetSwapTimer1()
	{	
		_timeSinceMovementModeSwap = 0f;
		_swapOverride = true;
		_rotationSmoothingOverride = true;
	}

	void Update()
	{
		_timeSinceMovementModeSwap += Time.deltaTime;
		if (_timeSinceMovementModeSwap > swapTimerDeacceleration)
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



			
			// old
			// if (!(cantGoUp || cantGoDown))
			// {
			// 	Quaternion newPitch = Quaternion.AngleAxis(targetAngle * Time.deltaTime, transformToPitch.right) * transformToYaw.rotation;
			//
			// 	//transformToPitch.rotation = Quaternion.RotateTowards(transformToPitch.rotation, targetRotation, speed * Time.fixedDeltaTime);
			// 	
			// 	transformToPitch.rotation = newPitch;
			// 	_lastPitchAngle = targetAngle;
			// }
			//
			
			//roll mesh
			// Quaternion bodyRoll = Quaternion.AngleAxis(-CurrentRightSpeed, transformToYaw.forward);
			// Quaternion targetRotationRoll = bodyRoll * transformToYaw.rotation * _originalBodyRotation;
			//
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



// 	int xzPoints = 8;
// int xyPoints = 8;

// Vector3 avoidWalls = Vector3.zero;
// Vector3 AvoidWallsVelocity()
//    {
//        Vector3 pos = transformToYaw.position;
//        
//        const float TAU = Mathf.PI * 2;
//
//        float xzAngleBetweenEachPoint = TAU / xzPoints;
//        float xyAngleBetweenEachPoint = TAU / xyPoints;
//        
//        Vector3[] xzRingPoints = new Vector3[xzPoints];
//        Vector3[] xyRingPoints = new Vector3[xyPoints];
//
//     velocityPushBackFromWalls = Vector3.zero;
//        
//        for (int i = 0; i < xzPoints; i++)
//        {
//            float x = Mathf.Cos(xzAngleBetweenEachPoint * i);
//            float z = Mathf.Sin(xzAngleBetweenEachPoint * i);
//
//            xzRingPoints[i] = new Vector3(x * radius + pos.x, pos.y, z * radius + pos.z);
//            Debug.DrawLine(pos, xzRingPoints[i], Color.red);
//            
//            var direction = new Vector3(x, 0f, z);
//            var hit = Physics.RaycastAll(pos, direction, radius, collisionLayer);
//
//            if (hit.Length <= 0) continue;
//            print("HIT");
//            
//            float distanceInToRay = radius - hit[0].distance; // it will be so rare with several hits that it's not worth it performance wise to sort them by distance
//
//            velocityPushBackFromWalls += -direction * distanceInToRay;
//        }
//        
//        for (int i = 0; i < xyPoints; i++)
//        {
//            float x = Mathf.Cos(xyAngleBetweenEachPoint * i);
//            float y = Mathf.Sin(xyAngleBetweenEachPoint * i);
//        
//            xyRingPoints[i] = new Vector3(x * radius + pos.x, y * radius + pos.y, pos.z);
//            Debug.DrawLine(pos, xyRingPoints[i], Color.green);
//
//            var direction = new Vector3(x, y, 0f);
//            if (Mathf.Approximately(Mathf.Abs( direction.x), 1f)) // there are already rays in these directions 
//            {
//                continue;
//            }
//            
//            RaycastHit[] hits = Physics.RaycastAll(pos, direction, radius, collisionLayer);
//
//            if (hits.Length <= 0) continue;
//            print("HIT");
//
//            float distanceInToRay = radius - hits[0].distance; 
//
//            velocityPushBackFromWalls += -direction * distanceInToRay;
//        }
//
//        return velocityPushBackFromWalls;
//    }