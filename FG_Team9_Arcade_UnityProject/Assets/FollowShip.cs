using UnityEngine;

public class FollowShip : MonoBehaviour
{
	public Transform playerYawTrasnform;

	void FixedUpdate()
	{
		transform.position = playerYawTrasnform.position;
	}
}