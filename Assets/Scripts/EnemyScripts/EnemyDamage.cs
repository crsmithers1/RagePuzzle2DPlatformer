using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
	public int health = 2;
	public AudioSource source;
	public AudioClip hit;
	public AudioClip die;
	public Rigidbody2D rb;
	[HideInInspector] public bool isDamaged;
	public ParticleSystem explosionPS;
	[HideInInspector] public bool isMoving;
	[HideInInspector] public bool isGrounded;
	[HideInInspector] public bool isKnockback;
	public Vector2 knockbackDistance;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
	{
		health -= damage;
		source.clip = hit;
		source.Play();
		isDamaged = true;
		Knockback();

		if (health <= 0)
		{
			source.clip = die;
			source.Play();
			CreateExplosion();
			Die();
		}
	}

	private void Knockback()
	{
    
		isKnockback = true;
		isGrounded = false;
		rb.velocity = new Vector2(knockbackDistance.x * -rb.velocity.x, knockbackDistance.y);
		
	}

	void Die()
	{
		Destroy(gameObject);
	}

	void CreateExplosion()
	{
		Instantiate(explosionPS, rb.transform.position, Random.rotation);
	}
}
