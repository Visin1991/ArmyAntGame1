using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageable {

	public float maxHealth = 100;
	public float health;
	protected bool dead;

	public virtual void Start()
	{
		health = maxHealth;
	}

	public virtual void TakeDamage(float damage)
	{
		health -= damage;
		if(health <=0 && !dead)
		{
			AudioManager.instance.PlaySound("EnemyDeath",transform.position);
			Die();
			return;
		}
		AudioManager.instance.PlaySound("Impact",transform.position);
	}

	public virtual void Die()
	{
		dead = true;
	}

}
