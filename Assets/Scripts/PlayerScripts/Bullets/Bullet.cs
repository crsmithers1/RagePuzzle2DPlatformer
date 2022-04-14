using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

	public float speed = 10f;
	public int damage = 1;
	public Rigidbody2D rb;
	public GameObject impactEffect;

	// Use this for initialization
	void Start()
	{
		rb.velocity = transform.right * speed;
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		BreakableObject bo = hitInfo.GetComponent<BreakableObject>();
		EnemyDamage enemyDamage = hitInfo.GetComponent<EnemyDamage>();
		if (bo != null)
		{
			bo.TakeDamage(damage);
		}
		else if (enemyDamage != null)
		{
			enemyDamage.TakeDamage(damage);
		}

		Instantiate(impactEffect, transform.position, transform.rotation);

		Destroy(gameObject);



	}
}