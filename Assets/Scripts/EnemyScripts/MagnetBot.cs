using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetBot : MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator anim;
	private int facingDirection = 1;
	private Vector2 damageTopRight;
	private Vector2 damageBotLeft;

	[Header("Enemy Stats")]
	public int health = 2;
	public float movementSpeed = 4;
	public float lastDamageTime;
	public float damageCooldown;
	public Vector2 knockbackDistance;
	private bool isKnockback;

	[Header("Collision Senses")]
	public Transform groundCheck;
	public float groundCheckDistance;
	public bool isGrounded;
	public bool isMoving;
	public LayerMask whatIsGround;
	public Transform wallCheck;
	public float wallCheckDistance;
	public bool isTouchingWall;
	public PlayerController playerController;
	public Transform damageCheck;
	public float damageHeight;
	public float damageWidth;
	public LayerMask whatIsPlayer;

	[Header("Audio & Particle Effects")]
	public AudioSource source;
	public AudioClip moveSmokeSound;
	public ParticleSystem moveSmoke;
	public AudioClip hit;
	public AudioClip die;

    #region Unity Callback Functions

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		facingDirection = 1;
    }

    private void Update()
    {
		CollisionSenses();
		EnemyMove();
		DealDamage();
		UpdateAnimations();

	}

	private void UpdateAnimations()
	{
		anim.SetBool("isMoving", isMoving);
		anim.SetBool("isKnockback", isKnockback);
	}
	#endregion

	#region Enemy AI

	void CollisionSenses()
    {
		isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

	void EnemyMove()
    {
        if(isGrounded && !isTouchingWall && !isKnockback)
        {
			rb.velocity = new Vector2(facingDirection * movementSpeed, 0.0f);
			isMoving = true;
			CreateSmoke();
        }
        if (isTouchingWall || !isGrounded)
        {
			Flip();
			isMoving = false;
        }
        if (isGrounded)
        {
			isKnockback = false;
        }
    }

	void DealDamage()
	{
        if (!playerController.isStomping)
        {
			if (Time.time >= lastDamageTime + damageCooldown)
			{
				damageBotLeft.Set(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
				damageTopRight.Set(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));
				Collider2D collision = Physics2D.OverlapArea(damageBotLeft, damageTopRight, whatIsPlayer);

				if (collision != null)
				{
					lastDamageTime = Time.time;
					playerController.DamageKnockBack();
					playerController.DecreaseEnergy();
				}
			}
        }
	}

	private void Flip()
    {
		facingDirection *= -1;
		transform.Rotate(0.0f, 180.0f, 0.0f);
	}

    #endregion

    #region Enemy Taking Damage Functions
    public void TakeDamage(int damage)
	{
		health -= damage;
		source.clip = hit;
		source.Play();
		Knockback();

		if (health <= 0)
		{
			source.clip = die;
			source.Play();
			Die();
		}
	}

	private void Knockback()
    {
		isKnockback = true;
		isGrounded = false;
		isMoving = false;
		rb.velocity = new Vector2(-knockbackDistance.x, knockbackDistance.y);
    }

	void Die()
	{
		Destroy(gameObject);
	}
	#endregion

	#region Audio & Particle Effects
	void CreateSmoke()
	{
		moveSmoke.Play();
		source.clip = moveSmokeSound;
		source.Play();
	}
	#endregion

	#region Other Functions
	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
		Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));

		Vector2 botLeft = new Vector2(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
		Vector2 botRight = new Vector2(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
		Vector2 topRight = new Vector2(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));
		Vector2 topLeft = new Vector2(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));

		Gizmos.DrawLine(botLeft, botRight);
		Gizmos.DrawLine(botRight, topRight);
		Gizmos.DrawLine(topRight, topLeft);
		Gizmos.DrawLine(topLeft, botLeft);
	}
    #endregion
}
