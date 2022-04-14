using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAirEnemyController : MonoBehaviour
{

    private int facingDirection = 1;
    //public Animator anim;
    private Vector2 damageTopRight;
    private Vector2 damageBotLeft;

    [Header("Enemy Stats")]
    public int flySpeed;
    public GameObject enemyPathStart;
    public GameObject enemyPathEnd;
    public float lastDamageTime;
    public float damageCooldown;
    public bool isMoving;
    public bool isKnockback;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Rigidbody2D rb;

    [Header("Collision Senses")]
    public PlayerController playerController;
    public Transform damageCheck;
    public float damageHeight;
    public float damageWidth;
    public LayerMask whatIsPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = enemyPathStart.transform.position;
        endPosition = enemyPathEnd.transform.position;
        StartCoroutine(Vector3LerpCoroutine(gameObject, endPosition, flySpeed));
        facingDirection = 0;
    }

    void Update()
    {
        DealDamage();
        Flip();
        UpdateAnimations();
        if (rb.position == endPosition)
        {
            StartCoroutine(Vector3LerpCoroutine(gameObject, startPosition, flySpeed));
        }
        if (rb.position == startPosition)
        {
            StartCoroutine(Vector3LerpCoroutine(gameObject, endPosition, flySpeed));
        }
        if (rb.velocity.x >= 0)
        {
            facingDirection = 1;
        }
        if (rb.velocity.x < 0)
        {
            facingDirection = -1;
        }
        if (!isKnockback)
        {
            isMoving = true;
        }
        if (isKnockback)
        {
            isMoving = false;
        }
    }

    private void UpdateAnimations()
    {
    //anim.SetBool("isMoving", isMoving);
    }

    IEnumerator Vector3LerpCoroutine(GameObject obj, Vector2 target, float speed)
    {
        Vector2 startPosition = obj.transform.position;
        float time = 0f;

        while (rb.position != target)
        {
            obj.transform.position = Vector2.Lerp(startPosition, target, (time / Vector2.Distance(startPosition, target)) * speed);
            time += Time.deltaTime;
            yield return null;
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
        if (facingDirection == -1)
        {
        transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 botLeft = new Vector2(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
        Vector2 botRight = new Vector2(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y - (damageHeight / 2));
        Vector2 topRight = new Vector2(damageCheck.position.x + (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));
        Vector2 topLeft = new Vector2(damageCheck.position.x - (damageWidth / 2), damageCheck.position.y + (damageHeight / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }
}