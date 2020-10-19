using System;
using System.Security.Principal;
using UnityEngine;

public enum GunBarrelsToUseForNewMovement
{
	All,
	Alternating
}

public class GattlingGunForNewMovement : MonoBehaviour
{
	[Tooltip("Cooldown in seconds")] public float cooldown;

	[Tooltip(
		"The transform's of the gun barrels (where the ammo will be shot from), the transform's forward direction is the direction the ammo will be shot towards")]
	public Transform[] gunBarrels;

	public GunBarrelsToUseForNewMovement gunBarrelsToUse;

	[NonSerialized] public bool shootIntent = false;

	int currentAlternatingIndex = 0;
	float timeOfLastShot = float.MinValue;

	Sfx _sfx;

	public void Awake()
	{
		_sfx = GetComponent<Sfx>();
	}

	public void Fire(Vector3 position, Vector3 direction)
	{
		shootIntent = true;
		pos = position;
		dir = direction;
	}

	Vector3 pos;
	Vector3 dir;
	
	void Update()
	{
		if (shootIntent && Time.time - timeOfLastShot > cooldown)
		{
			shootIntent = false;
			print("FIRE!");

			timeOfLastShot = Time.time;

			//_sfx.PlayShootSoundRandomPitch(1.1f, 1.2f);

			GameObject currentShot;

			currentShot = ProjectilePool.Instance.GetPooledObject(ProjectileType.GattlingProjectile);
			currentShot.transform.SetPositionAndRotation(pos, Quaternion.Euler(dir));
			currentShot.SetActive(true);
		}
	}
}