using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public enum FireMode{Auto,Burst,Single};
	public FireMode fireMode;

	public Transform[] muzzles;
	public Bullet projectile;

	[Header("Main")]
	public float msBetweenShots = 100;
	public float muzzleVelocity = 35; //shot frequency
	public int burstCount = 3;
	float nextShootTime;
	public float reloadTime = 0.3f;

	[Header("Effects")]
	public Transform shell;
	public Transform shellEjection;
	MuzzleFlash muzzleFlash;
	public AudioClip shootAudio;
	public AudioClip reloadAudio;


	bool triggerReleasedSinceLastShot;
	int shootsRemainingInBurst;

	bool isReloading;
	int projectilesRemainingInMag;
	public int projectilesPerMag = 12;

	[Header("Recoil")]
	public float recoilMoveSettleTime = 0.1f;
	public float recoilRotionSettleTime = 0.2f;
	float recoilAngle = 0.0f;
	public Vector2 recoilMoveMinMax = new Vector2(0.01f,0.3f);
	public Vector2 recoilAngleMinMax = new Vector2(1,5);
	Vector3 recoilSmoothDampVelocity;
	float recoilRotationDegreeSmoothDampVelocity;

	void Start()
	{
		shootsRemainingInBurst = burstCount;
		muzzleFlash = GetComponent<MuzzleFlash>();
		projectilesRemainingInMag = projectilesPerMag;
	}
		

	void LateUpdate()
	{
		//animate recoil
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition,Vector3.zero,ref recoilSmoothDampVelocity,recoilMoveSettleTime);
		recoilAngle = Mathf.SmoothDamp(recoilAngle,0,ref recoilRotationDegreeSmoothDampVelocity,recoilRotionSettleTime);
		if(!isReloading)
		{
			transform.localEulerAngles = Vector3.zero + Vector3.left * recoilAngle;
			if(projectilesRemainingInMag == 0){
				Reload();
			}
		}
	}

	void Shoot()
	{
		if(!isReloading && Time.time > nextShootTime && projectilesRemainingInMag > 0)
		{
			if(fireMode == FireMode.Burst)
			{
				if(shootsRemainingInBurst == 0)
				{
					return;
				}
				shootsRemainingInBurst --;
			}else if(fireMode == FireMode.Single)
			{
				if(!triggerReleasedSinceLastShot)
				{
					return;
				}
			}

			for(int i = 0;i<muzzles.Length; i++)
			{
				if(projectilesRemainingInMag == 0)
				{
					break;
				}
				projectilesRemainingInMag --;
				nextShootTime = Time.time + msBetweenShots / 1000;
				Bullet newProjectile = Instantiate(projectile,muzzles[i].position,muzzles[i].rotation) as Bullet;
				newProjectile.SetSpeed(muzzleVelocity);
			}

			Instantiate(shell,shellEjection.position,shellEjection.rotation);
			muzzleFlash.Activate();
			transform.localPosition -= Vector3.forward * Random.Range(recoilMoveMinMax.x,recoilMoveMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x,recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp(recoilAngle,0,20);

			AudioManager.instance.PlaySound(shootAudio,transform.position);

		}
	}

	public void Reload()
	{
		if(!isReloading){
			StartCoroutine(AnimateReload());
			AudioManager.instance.PlaySound(reloadAudio,transform.position);
		}

	}

	IEnumerator AnimateReload()
	{
		isReloading = true;
		yield return new WaitForSeconds(.2f);

		float reloadSpeed = 1.0f / reloadTime;
		float percent = 0;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 30.0f;

		while(percent <1)
		{
			percent += Time.deltaTime * reloadSpeed;

			float interpolation = (-Mathf.Pow(percent,2) + percent) * 4; //(-x^2 + x) * 4 => -4*x^2 + 4x; //顶点为 ［1／2，1］ 且 过原点的一元二次函数。二元一次顶点函数为：［－b/2a,(4ac-b^2)/4a].
			float reloadAngle = Mathf.Lerp(0,maxReloadAngle,interpolation);
			transform.localEulerAngles = initialRot + (Vector3.left - Vector3.up) * reloadAngle;
			//Debug.Log(transform.localEulerAngles);
			yield return null;
		}
		isReloading = false;
		projectilesRemainingInMag = projectilesPerMag;
	}

	public void OnTriggerHold()
	{
		Shoot();
		triggerReleasedSinceLastShot = false;
	}
	public void OnTriggerRelease()
	{
		triggerReleasedSinceLastShot = true;
		shootsRemainingInBurst = burstCount;
	}

}
