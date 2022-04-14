using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderBot: MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator anim;
	private int facingDirection = 1;
	private Vector2 damageTopRight;
	private Vector2 damageBotLeft;
	private Vector2 forkTopRight;
	private Vector2 forkBotLeft;

	[Header("Enemy Stats")]
	public float movementSpeed = 1;
	public float lastDamageTime;
	public float damageCooldown;
	private bool isKnockback;
	private bool isDamaged;
	public EnemyDamage enemyDamage;
	private bool isForkLift;
	public float forkLiftSpeed;

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
	public Transform forkCheck;
	public float forkWidth;
	public float forkHeight;
	public Rigidbody2D playerRB;

	[Header("Audio & Particle Effects")]
	public AudioSource source;
	public AudioClip hit;
	public AudioClip die;
	public ParticleSystem moveSmoke;
	public AudioClip moveSmokeSound;

    #region Unity Callback Functions

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
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
		ForkLiftThrow();
		UpdateAnimations();

	}

    private void UpdateAnimations()
    {
		anim.SetBool("isMoving", isMoving);
		anim.SetBool("isKnockback", isKnockback);
		anim.SetBool("isForkLift", isForkLift);
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
		if (!isDamaged)
		{

			if (isGrounded && !isTouchingWall && !isKnockback)
			{
				rb.velocity = new Vector2(facingDirection * movementSpeed, 0.0f);
				isMoving = true;

				CreateSmoke();
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

	private void ForkLiftThrow()
    {
		forkBotLeft.Set(forkCheck.position.x - (forkWidth / 2), forkCheck.position.y - (forkHeight / 2));
		forkTopRight.Set(forkCheck.position.x + (forkWidth / 2), forkCheck.position.y + (forkHeight / 2));

		Collider2D other = Physics2D.OverlapArea(forkBotLeft, forkTopRight, whatIsPlayer);

		if (other != null)
        {
			Debug.Log("ForkLift Collision");
			playerRB.velocity = new Vector2(playerRB.velocity.x, forkLiftSpeed);
			isForkLift = true;
		}
        else
        {
			isForkLift = false;
        }
    }

	private void Flip()
    {
		facingDirection *= -1;
		transform.Rotate(0.0f, 180.0f, 0.0f);
	}

	#endregion

	#region Other Functions
	void CreateSmoke()
	{
		moveSmoke.Play();
		source.clip = moveSmokeSound;
		source.Play();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
		Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));

		Vector2 botLeft = new Vector2(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
		Vector2 botRight = new Vector2(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
		Vector2 topRight = new Vector2(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));
		Vector2 topLeft = new Vector2(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));

		Vector2 forkBotLeft = new Vector2(forkCheck.position.x - (forkWidth / 2), forkCheck.position.y - (forkHeight / 2));
		Vector2 forkBotRight = new Vector2(forkCheck.position.x + (forkWidth / 2), forkCheck.position.y - (forkHeight / 2));
		Vector2 forkTopRight = new Vector2(forkCheck.position.x + (forkWidth / 2), forkCheck.position.y + (forkHeight / 2));
		Vector2 forkTopLeft = new Vector2(forkCheck.position.x - (forkWidth / 2), forkCheck.position.y + (forkHeight / 2));

		Gizmos.DrawLine(botLeft, botRight);
		Gizmos.DrawLine(botRight, topRight);
		Gizmos.DrawLine(topRight, topLeft);
		Gizmos.DrawLine(topLeft, botLeft);
		Gizmos.DrawLine(forkBotLeft, forkBotRight);
		Gizmos.DrawLine(forkBotRight, forkTopRight);
		Gizmos.DrawLine(forkTopRight, forkTopLeft);
		Gizmos.DrawLine(forkTopLeft, forkBotLeft);
	}
    #endregion
}
