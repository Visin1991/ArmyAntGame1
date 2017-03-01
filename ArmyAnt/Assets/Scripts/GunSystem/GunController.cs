using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

	public Transform GunHolder;
	public Gun[] allGuns;
	Gun equippedGun;


	void Start()
	{
		EquipGun(allGuns[2]);
	}

	public void EquipGunIndex(int index)
	{
		EquipGun(allGuns[index]);
	}

	void EquipGun(Gun gun)
	{
		if(equippedGun != null)
		{
			Destroy(equippedGun.gameObject);
		}
		equippedGun = Instantiate(gun,GunHolder.position,GunHolder.rotation) as Gun;
		equippedGun.transform.SetParent(GunHolder);
	}

	public void OnTriggerHold()
	{
		if(equippedGun != null)
		{
			equippedGun.OnTriggerHold();
		}
	}

	public void OntriggerRelease()
	{
		if(equippedGun != null)
		{
			equippedGun.OnTriggerRelease();
		}
	}

	public void Reload()
	{
		if(equippedGun != null)
		{
			equippedGun.Reload();
		}
	}

}
