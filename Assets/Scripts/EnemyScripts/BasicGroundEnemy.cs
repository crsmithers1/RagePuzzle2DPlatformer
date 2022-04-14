using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGroundEnemyTemplate : MonoBehaviour
{
	private Rigidbody2D rb;
	//private Animator anim;
	private int facingDirection = 1;
	private Vector2 damageTopRight;
	private Vector2 damageBotLeft;

	[Header("Enemy Stats")]
	public float movementSpeed = 1;
	public float lastDamageTime;
	public float damageCooldown;
	private bool isKnockback;
	private bool isDamaged;
	public EnemyDamage enemyDamage;

	[Header("Collision Senses")]
	public Transform groundCheck;
	public float groundCheckDistance;
	public bool isGrounded;
	public bool wasGrounded;
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
	public AudioClip hit;
	public AudioClip die;

	#region Unity Callback Functions

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		//anim = GetComponent<Animator>();
		facingDirection = 1;
	}

	private void Update()
	{
		isGrounded = enemyDamage.isGrounded;
		isDamaged = enemyDamage.isDamaged;
		isKnockback = enemyDamage.isKnockback;
		CollisionSenses();
		EnemyMove();
		DealDamage();
		//UpdateAnimations();

	}

	//private void UpdateAnimations()
	//{
	//anim.SetBool("isMoving", isMoving);
	//anim.SetBool("isKnockback", isKnockback);
	//}
	#endregion

	#region Enemy AI

	void CollisionSenses()
	{
		isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
		isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
	}

	void EnemyMove()
	{
		if (!isDamaged)
		{

			if (isGrounded && !isTouchingWall && !isKnockback)
			{
				rb.velocity = new Vector2(facingDirection * movementSpeed, 0.0f);
				isMoving = true;

				//CreateSmoke();
			}
			if (isTouchingWall || !isGrounded && !isKnockback)
			{
				Flip();
				isMoving = false;
			}
			if (isGrounded && wasGrounded)
			{

				enemyDamage.isGrounded = true;
				enemyDamage.isKnockback = false;
				wasGrounded = false;

			}
		}
		else if (isDamaged && !isGrounded)
		{
			isDamaged = false;
			enemyDamage.isDamaged = false;
			wasGrounded = true;

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
