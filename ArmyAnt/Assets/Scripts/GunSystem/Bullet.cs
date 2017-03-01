using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public LayerMask collisionMask;
	public float lifeTime = 3;
	float speed =  10.0f;
	float damage = 1;

	void Start()
	{
		StartCoroutine(DestroyBullet());
	}

	public void SetSpeed(float _speed)
	{
		speed = _speed;
	}

	void Update()
	{
		transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}

	void CheckCollisions(float moveDistance)
	{
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit hitInfo;

		if(Physics.Raycast(ray, out hitInfo, moveDistance,collisionMask,QueryTriggerInteraction.Collide))
		{
			OnHitObject(hitInfo);
		}
	}

	void OnHitObject(RaycastHit hitInfo)
	{
		IDamageable damageableObject = hitInfo.collider.GetComponent<IDamageable>();
		if(damageableObject != null)
		{
			damageableObject.TakeDamage(damage);
		}
		GameObject.Destroy(gameObject);
	}

	IEnumerator DestroyBullet()
	{
		yield return new WaitForSeconds(lifeTime);
		GameObject.Destroy(this.gameObject);
	}
}
