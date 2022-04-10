using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
	public int health = 2;
	public AudioSource source;
	public AudioClip hit;
	public AudioClip breakObject;

	public void TakeDamage(int damage)
	{
		health -= damage;
		source.clip = hit;
		source.Play();

		if (health <= 0)
		{
			source.clip = breakObject;
			source.Play();
			Die();
		}
	}

	void Die()
	{
		Destroy(gameObject);
	}

}

