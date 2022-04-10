using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables & Components
    private float movementInputDirection;
    private float lookInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastDash = -100f;

    public int EnergyLeft { get; private set; }
    [HideInInspector] public int facingDirection = 1;
    private int lastWallJumpDirection;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Developer Mode")]
    [Tooltip("Removes Energy & Time Requirements - Only Chris & Cheaters are allowed this")] public bool devMode;

    [Header("Movement Settings")]
    [Tooltip("The speed the Character moves")] public float movementSpeed = 4.0f;
    [Tooltip("The max amount of abilities the Character can use")] public int maxEnergy = 2;
    [Tooltip("The power of a full press jump")] public float jumpForce = 6.0f;
    [Tooltip("The amount of time the dash ability takes")] public float dashTime = 0.25f;
    [Tooltip("The speed which the dash ability travels")] public float dashSpeed = 20f;
    [Tooltip("The amount of time between dash ability uses")] public float dashCoolDown = 1f;
    [Tooltip("The speed the Character slows down in air without input")] public float airDragMultiplier = 0.95f;
    [Tooltip("The speed the Character falls")] public float fallingGravity = 4f;
    [Tooltip("The power of a tap press jump")] public float variableJumpHeightMultiplier = 0.5f;
    [Tooltip("The power of a wall jump")] public float wallJumpForce = 4.0f;
    [Tooltip("The speed the Character slides down the wall")] public float wallSlideSpeed;
    [Tooltip("The amount of time you leave between spam pressing jumps")] public float jumpTimerSet = 0.15f;
    [Tooltip("The time in which you leave between turns")] public float turnTimerSet = 0.1f;
    [Tooltip("The time between wall jumps")] public float wallJumpTimerSet = 0.5f;
    [Tooltip("The direction a wall jump travels")] public Vector2 wallJumpDirection;

    [Header("Attack Settings")]
    [Tooltip("The location where the bullet is created when firing horizontal")] public Transform firePointHorizontal;
    [Tooltip("The location where the bullet is created when firing Up")] public Transform firePointUp;
    [Tooltip("The location where the bullet is created when firing down")] public Transform firePointDown;
    [Tooltip("The location where the bullet is created when firing diagonal")] public Transform firePointDUp;
    [Tooltip("The bullet prefab")] public GameObject bulletPrefab;
    [Tooltip("The bullet up prefab")] public GameObject bulletUpPrefab;
    [Tooltip("The bullet diagonal right up prefab")] public GameObject bulletDRUpPrefab;
    [Tooltip("The bullet diagonal left up prefab")] public GameObject bulletDLUpPrefab;
    [Tooltip("The bullet down prefab")] public GameObject bulletDownPrefab;
    [Tooltip("The recoil amount when shot")] public float blastKnockback = 10f;
    [Tooltip("The distance the character goes back after receiving damage")] public Vector2 damageKnockBack;
    [Tooltip("The damage amount when stomping")] public int stompDamage = 1;
    [Tooltip("The bounce amount after a stomp")] public float stompBounce = 5f;

    [Header("States")]
    [Tooltip("Is the Character walking")][SerializeField] private bool isWalking;
    [Tooltip("Is the Character touching the ground")][SerializeField] private bool isGrounded;
    [Tooltip("Is the Character touncking the floor")][SerializeField] private bool isTouchingWall;
    [Tooltip("Is the Character wall sliding")][SerializeField] private bool isWallSliding;
    [Tooltip("Is the Character shooting")][SerializeField] private bool isShooting;
    [Tooltip("Is the Character shooting up")][SerializeField] private bool isShootingUp;
    private bool isShootingDUp;
    [Tooltip("Is the Character shooting Down")][SerializeField] private bool isShootingDown;
    [Tooltip("Is the Character falling downwards")][SerializeField] private bool isFalling;
    [Tooltip("Is the Character using the dash ability")][SerializeField] private bool isDashing;
    [Tooltip("Is the Character too close to a wall to dash")][SerializeField] private bool isNotAtDashDistance;
    private bool isFacingRight = true;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    public bool isStomping;
    private readonly bool isKnockback;
    public bool isLight;
    public bool isAimDR;
    public bool isAimDL;

    [Header("Particle & Sound Effects")]
    [Tooltip("This is the Light component from the Characters Head")] public GameObject headLight;
    [Tooltip("This is the dust kicked up by turning")] public ParticleSystem turnDust;
    [Tooltip("This is the trail from jumping")] public ParticleSystem trailRight;
    [Tooltip("This is the trail from jumping")] public ParticleSystem trailLeft;
    [Tooltip("This is the trail from shooting")] public ParticleSystem trailBullet;
    [Tooltip("This is the trail from shooting Up")] public ParticleSystem trailBulletUp;
    [Tooltip("This is the trail from shooting Down")] public ParticleSystem trailBulletDown;
    [Tooltip("This is the Energy Particles when damaged")] public ParticleSystem damagePS;
    [Tooltip("This is the trail from dashing")] public ParticleSystem trailDash;
    [Tooltip("The audio component playing the sounds")] public AudioSource source;
    [Tooltip("The audio clip when the player turns")] public AudioClip turnDustSound;
    [Tooltip("The audio clip when the player is jumping/dashing")] public AudioClip jumpRocketSound;
    [Tooltip("The audio clip when the player is shooting")] public AudioClip shootSound;

    [Header("Collision Senses")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public Transform wallCheck;
    public float wallCheckDistance;
    public Transform dashCheck;
    public float dashCheckDistance;
    public LayerMask whatIsGround;


    [Header("Other Settings")]
    public ControlCenter controlCenter;
    #endregion
    #region Unity Callbacks
    void Start()
    {
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        EnergyLeft = maxEnergy;
        wallJumpDirection.Normalize();
        isLight = true;
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanBoost();
        CheckIfWallSliding();
        CheckJump();
        CheckIfFalling();
        CheckDash();
        CheckAimDR();
        CheckAimDL();
        Light();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<MagnetBot>().TakeDamage(stompDamage);
            isStomping = true;
            Debug.Log("Stomp");
            StompBounce();
        }
        else
        {
            isStomping = false;
        }
    }

    #endregion
    #region Check Functions
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isNotAtDashDistance = Physics2D.Raycast(dashCheck.position, transform.right, dashCheckDistance, whatIsGround);
    }
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckIfFalling()
    {
        if(!isGrounded && !isWallSliding && rb.velocity.y < 0)
        {
            isFalling = true;
            rb.gravityScale = fallingGravity;
        }
        else if(isGrounded)
        {
            isFalling = false;
            rb.gravityScale = 1f;
        }
        else if (!isGrounded && Input.GetButtonDown("Jump"))
        {
            isFalling = false;
            rb.gravityScale = 1f;
        }
        else if(!isGrounded && isKnockback)
        {
            isFalling = false;
            rb.gravityScale = 1f;
        }
        else if (isShooting)
        {
            isFalling = false;
            rb.gravityScale = 1f;
        }
        else if (isShootingDown)
        {
            isFalling = false;
            rb.gravityScale = 1f;
        }
    }

    private void CheckIfCanBoost()
    {
        if (devMode == true)
        {
            EnergyLeft = maxEnergy;
        }

        if (isTouchingWall)
        {
            checkJumpMultiplier = false;
            canWallJump = true;
        }

        if (EnergyLeft <= 0)
        {
            canNormalJump = false;
            canWallJump = false;
        }
        else
        {
            canNormalJump = true;
            canWallJump = true;
        }
    }
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0)
        {
            isWallSliding = true;
            Debug.Log("Wall Sliding");
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                isFalling = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0.0f);
                dashTimeLeft -= Time.deltaTime;
            }

            if (dashTimeLeft <= 0 || isTouchingWall || isNotAtDashDistance)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    private void CheckAimDR()
    {
        if(facingDirection == 1)
        {
            if (Input.GetButton("AimDR"))
            {
                isAimDR = true;
                headLight.transform.eulerAngles = Vector3.forward * -45;
                Debug.Log("Aim Diagonal Right");
            }
            else
            {
                if (isAimDL == false)
                {
                    headLight.transform.eulerAngles = new Vector3(0, 0, 90 * facingDirection);
                }
                isAimDR = false;
            }
        }
        else if(facingDirection == -1)
        {
            if (Input.GetButton("AimDR"))
            {
                Flip();
                isAimDR = true;
                    headLight.transform.eulerAngles = Vector3.forward * -45;
                Debug.Log("Aim Diagonal Right");
            }
            else
            {
                isAimDR = false;
                if(isAimDL == false)
                {
                headLight.transform.eulerAngles = new Vector3(0, 0, 90 * facingDirection);
                }
            }
        }
    }

    private void CheckAimDL()
    {
        if(facingDirection == -1)
        {
            if (Input.GetButton("AimDL"))
            {
            isAimDL = true;
                headLight.transform.eulerAngles = Vector3.forward * 45;
                Debug.Log("Aim Diagonal Left");
            }
            else
            {
            isAimDL = false;
                if(isAimDR == false)
                {
                headLight.transform.eulerAngles = new Vector3(0, 0, -90 * facingDirection);
                }
            }
        }
        if (facingDirection == 1)
        {
            if (Input.GetButton("AimDL"))
            {
                Flip();
                isAimDL = true;
                headLight.transform.eulerAngles = Vector3.forward * 45;
                Debug.Log("Aim Diagonal Left");
            }
            else
            {
                isAimDL = false;
                if (isAimDR == false)
                {
                    headLight.transform.eulerAngles = new Vector3(0, 0, -90 * facingDirection);
                }
            }
        }
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        lookInputDirection = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (EnergyLeft > 0 && !isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (movementInputDirection != 0 && !isKnockback && !isTouchingWall)
        {
            canMove = true;
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }


        if (lookInputDirection != -1 && !isAimDL && !isAimDR && Input.GetButtonDown("Fire1") && isGrounded && EnergyLeft > 0)
        {
            isShooting = true;
            isShootingUp = false;
            isShootingDown = false;
            isShootingDUp = false;
            Shoot();
            Debug.Log("Shoot Horizontal");
        }
        else if (lookInputDirection == -1 && !isAimDL && !isAimDR && Input.GetButtonDown("Fire1") && isGrounded && EnergyLeft > 0)
        {
            isShooting = false;
            isShootingUp = true;
            isShootingDown = false;
            isShootingDUp = false;
            ShootUp();
            Debug.Log("ShootUP");
        }
        else if (isAimDR && !isAimDL && Input.GetButtonDown("Fire1") && isGrounded && EnergyLeft > 0)
        {
            isShooting = false;
            isShootingUp = false;
            isShootingDown = false;
            isShootingDUp = true;
            ShootDRUp();
            Debug.Log("ShootDRUP");
        }
        else if (isAimDL && !isAimDR && Input.GetButtonDown("Fire1") && isGrounded && EnergyLeft > 0)
        {
            isShooting = false;
            isShootingUp = false;
            isShootingDown = false;
            isShootingDUp = true;
            ShootDLUp();
            Debug.Log("ShootDLUP");
        }
        else if (lookInputDirection != 1 && Input.GetButtonDown("Fire1") && !isGrounded && EnergyLeft > 0)
        {
            rb.velocity = new Vector2(blastKnockback * 2, rb.velocity.y);
            isShooting = true;
            isShootingUp = false;
            isShootingDown = false;
            isShootingDUp = false;
            isFalling = false;
            Shoot();
            Debug.Log("AirShoot");
        }
        else if (lookInputDirection == 1 && Input.GetButtonDown("Fire1") && !isGrounded && EnergyLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, blastKnockback);
            isShooting = false;
            isShootingUp = false;
            isShootingDown = true;
            isShootingDUp = false;
            isFalling = false;
            ShootDown();
            Debug.Log("ShootDown");
        }
        else
        {
            isShooting = false;
            isShootingUp = false;
            isShootingDown = false;
            isShootingDUp = false;
        }

        if (Input.GetButtonDown("Dash") && !isNotAtDashDistance && EnergyLeft > 0)
        {
            if (Time.time >= (lastDash + dashCoolDown))
            {
                EnergyLeft--;
                AttemptToDash();
                CreateDashTrail();
                Debug.Log("Dash");
            }
        }

        if (Input.GetButtonDown("Reset"))
        {
            controlCenter.ResetScene();
        }
    }
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }
    #endregion
    #region Set Functions
    public int GetFacingDirection()
    {
        return facingDirection;
    }
    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }

        if (isShooting)
        {
            rb.velocity = new Vector2(-facingDirection * blastKnockback, 0);
        }

        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

        if (isFalling)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
    }
    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            EnergyLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            CreateRightTrail();
            CreateLeftTrail();
            Debug.Log("Normal Jump");
        }
    }
    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            EnergyLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
            CreateRightTrail();
            CreateLeftTrail();
            Debug.Log("Wall Jump");
        }
    }

    private void Light()
    {
        if (Input.GetButtonDown("Light") && !isLight)
        {
            headLight.SetActive(true);
            isLight = true;
            Debug.Log("Toggle Light On");
        }
        else if (Input.GetButtonDown("Light") && isLight)
        {
            headLight.SetActive(false);
            isLight = false;
            Debug.Log("Toggle Light Off");
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        isFalling = false;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }
    private void Flip()
    {
        if (!isWallSliding && isGrounded && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
            CreateDust();
        }
        else if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }
    #endregion
    #region Combat Functions

    private void Shoot()
    {
        if ((EnergyLeft > 0) && !isWallSliding)
        {
            Instantiate(bulletPrefab, firePointHorizontal.position, firePointHorizontal.rotation);
            CreateBulletTrail();
            EnergyLeft--;
        }
    }

    private void ShootUp()
    {
        if ((EnergyLeft > 0) && !isWallSliding)
        {
            Instantiate(bulletUpPrefab, firePointUp.position, firePointUp.rotation);
            CreateBulletUpTrail();
            EnergyLeft--;
        }
    }

    private void ShootDRUp()
    {
        if ((EnergyLeft > 0) && !isWallSliding)
        {
            Instantiate(bulletDRUpPrefab, firePointDUp.position, firePointDUp.rotation);
            //CreateBulletDUpTrail();
            EnergyLeft--;
        }
    }

    private void ShootDLUp()
    {
        if ((EnergyLeft > 0) && !isWallSliding)
        {
            Instantiate(bulletDLUpPrefab, firePointDUp.position, firePointDUp.rotation);
            //CreateBulletDUpTrail();
            EnergyLeft--;
        }
    }

    private void ShootDown()
    {
        if ((EnergyLeft > 0) && !isWallSliding)
        {
            Instantiate(bulletDownPrefab, firePointDown.position, firePointDown.rotation);
            CreateBulletDownTrail();
            EnergyLeft--;
        }
    }

    public void DamageKnockBack()
    {
        if (!isStomping)
        {
        rb.velocity = new Vector2(damageKnockBack.x * -facingDirection, damageKnockBack.y);
        canMove = false;
        Debug.Log("Damaged");
        CreateDamageParticles();
        }
    }

    private void StompBounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, stompBounce);
        Debug.Log("Stomp");
    }

    #endregion
    #region Energy Functions
    public void IncreaseEnergy()
    {
        if(EnergyLeft < maxEnergy)
        {
        EnergyLeft++;
        }
    }

    public void DecreaseEnergy()
    {
        if(EnergyLeft > 0)
        {
        EnergyLeft--;
        }
    }

    public void IncreaseEnergyToMax()
    {
        if (EnergyLeft < maxEnergy)
        {
            EnergyLeft = maxEnergy;
        }
    }

    #endregion
    #region Particle Effects & Audio

    void CreateDust()
    {
        if((!isAimDL && movementInputDirection >= 0) || (!isAimDR && movementInputDirection <= 0))
        {
        turnDust.Play();
        source.clip = turnDustSound;
        source.Play();

        }
    }

    void CreateRightTrail()
    {
        trailRight.Play();
        source.clip = jumpRocketSound;
        source.Play();
        //CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
    }
    void CreateLeftTrail()
    {
        trailLeft.Play();
    }
    void CreateBulletTrail()
    {
        trailBullet.Play();
        source.clip = shootSound;
        source.Play();
        //CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
    }
    void CreateBulletUpTrail()
    {
        trailBulletUp.Play();
        source.clip = shootSound;
        source.Play();
        //CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
    }
    void CreateBulletDownTrail()
    {
        trailBulletDown.Play();
        source.clip = shootSound;
        source.Play();
        //CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
    }

    void CreateDamageParticles()
    {
        damagePS.Play();
    }
    void CreateDashTrail()
    {
        trailDash.Play();
        source.clip = jumpRocketSound;
        source.Play();
        //CinemachineShake.Instance.ShakeCamera(1f, 0.05f);
    }

    #endregion
    #region Other Functions
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isShooting", isShooting);
        anim.SetBool("isShootingUp", isShootingUp);
        anim.SetBool("isShootingDUp", isShootingDUp);
        anim.SetBool("isShootingDown", isShootingDown);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isKnockback", isKnockback);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(dashCheck.position, new Vector3(dashCheck.position.x + dashCheckDistance * facingDirection, dashCheck.position.y, dashCheck.position.z));
    }
    #endregion
}
