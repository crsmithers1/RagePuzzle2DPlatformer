using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUp : MonoBehaviour
{

	public float speed = 20f;
	public int damage = 40;
	public Rigidbody2D rb;
	public GameObject impactEffect;

	// Use this for initialization
	void Start()
	{
		rb.velocity = new Vector2(0, speed);
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		BreakableObject bo = hitInfo.GetComponent<BreakableObject>();
		MagnetBot enemy = hitInfo.GetComponent<MagnetBot>();
		if (bo != null)
		{
			bo.TakeDamage(damage);
		}
		else if (enemy != null)
		{
			enemy.TakeDamage(damage);
		}

		Instantiate(impactEffect, transform.position, transform.rotation);

		Destroy(gameObject);



	}
}