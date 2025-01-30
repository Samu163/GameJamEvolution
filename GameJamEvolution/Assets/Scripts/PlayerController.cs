using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Player Movement")]
    private float movHorizontal = 0f;
    [SerializeField] private float movSpeed;
    [SerializeField] private float movSpeedRunning;
    private float currentMovSpeed;
    [Range(0, 2f)]
    [SerializeField] private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    private bool isLookingRight = true;
    public PhysicMaterial playerPhysicMaterial;

    [Header("Player Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private float radiusSize;
    [SerializeField] private bool isGrounded;
    private bool jump = false;
    private bool isJumping = true;
    [SerializeField] private float gravityUp;
    [SerializeField] private float gravityDown;
    [SerializeField] private float gravity;

    [Header("Player Wall Jump")]
    [SerializeField] private float wallJumpForce;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector3 boxSizeWall;
    [SerializeField] private float radiusWallCheckSize;
    [SerializeField] private bool isTouchingWall;
    [SerializeField] private bool canWallJump = true;
    private bool wallJump = false;
    private bool isWallJumping = false;

    [Header("Player Death")]
    [SerializeField] private Vector3 startPosition;

    private int freezeTimer = 5;
    private int freezeTimerCounter = 0;
    private bool canFreeze = false;

    [SerializeField] private  Animator animator;

    private float lastFootstepTime = 0f;
    private float footstepCooldown = 0.3f; // Adjust this value to control footstep frequency

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        animator.SetBool("isWalking", false);
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            rb.MovePosition(startPosition);
        }
        //isGrounded = Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, groundLayer);
        isGrounded = Physics.CheckSphere(groundCheck.position, radiusSize, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
        isTouchingWall = Physics.CheckSphere(wallCheck.position, radiusWallCheckSize, wallLayer);

        if (rb.velocity.y < 0)
        {
            animator.SetBool("isJumping", false);
        }

        movHorizontal = Input.GetAxisRaw("Horizontal") * movSpeedRunning;

        


        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            wallJump = true;
            isJumping = true;
            canFreeze = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
        }

        

        if (movHorizontal != 0 && !isTouchingWall && isGrounded)
        {
            animator.SetBool("isRunning", true);
        }
        else if (movHorizontal == 0 || isTouchingWall)
        {
            animator.SetBool("isRunning", false);
        }

        if (isTouchingWall && !isGrounded)
        {
            animator.SetBool("isWallHanging", true);
            animator.SetBool("isJumping", false);
        }
        else if (!isTouchingWall && isGrounded)
        {
            animator.SetBool("isWallHanging", false);
        }
    
        
        if (isGrounded && !isJumping && canFreeze)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionZ;
        }

    }
    
    private void FixedUpdate()
    {
        
        

        MovePlayer(movHorizontal * Time.fixedDeltaTime, jump, wallJump);
        if (isJumping && !isGrounded && !isTouchingWall && rb.velocity.y < 0.5f)
        {
            gravity = Mathf.Lerp(gravityUp, gravityDown, Time.fixedDeltaTime);
        }
        

        if (rb.velocity.y <= 0 && isTouchingWall)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - 0.1f, rb.velocity.z);
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity, rb.velocity.z);
        }


            jump = false;
        wallJump = false;
    }

    private void PlayFootstepSound()
    {
        if (isGrounded && Mathf.Abs(movHorizontal) > 0.1f)
        {
            // Increased cooldown times for both running and walking
            float currentCooldown = (currentMovSpeed == movSpeedRunning) ? 0.4f : 0.6f;
            
            if (Time.time - lastFootstepTime >= currentCooldown)
            {
                if (SFXManager.Instance != null)
                {
                    float volumeMultiplier = Mathf.Clamp01(Mathf.Abs(movHorizontal) / currentMovSpeed);
                    SFXManager.Instance.PlayEffect("Movement", volumeMultiplier);
                    lastFootstepTime = Time.time;
                }
            }
        }
    }

    private void MovePlayer(float move, bool jumping, bool wallJumping)
    {
        Vector3 targetVelocity = new Vector3(movHorizontal, rb.velocity.y, 0);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothTime);

        PlayFootstepSound();

        if (movHorizontal > 0 && !isLookingRight)
        {
            Flip();
        }
        else if (movHorizontal < 0 && isLookingRight)
        {
            Flip();
        }

        // Wall Jump logic and sound
        if (wallJumping && isTouchingWall && !isGrounded && canWallJump && !isWallJumping)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isTouchingWall = false;
            canWallJump = false;
            isWallJumping = true;
            animator.SetBool("isWallJumping", true);
            animator.SetBool("isWallHanging", false);
            
            // Play wall jump sound
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayEffect("WallJump", 1f);
            }
        }
        // Regular Jump logic and sound - only if not wall jumping
        else if (jumping && isGrounded && !isWallJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetBool("isJumping", true);
            animator.SetBool("isWallJumping", false);

            // Play jump sound
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayEffect("Jump", 1f);
            }
        }
    }

    private void Flip()
    {
        isLookingRight = !isLookingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, radiusSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCheck.position, radiusWallCheckSize);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ActivateRespawnEffects();
            }

            // Play both death sounds
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayEffect("Death", 1f);
                SFXManager.Instance.PlayEffect("DeathImpact", 1f);
            }
        }

        if (collision.collider.CompareTag("SlideGround"))
        {
            smoothTime = 1f;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("isWallHanging", false);
            isJumping = false;
            isWallJumping = false;
            canWallJump = true;
            animator.SetBool("canJump", true);

            // Play landing sound
            if (SFXManager.Instance != null)
            {
                // Volume based on vertical velocity
                float volumeMultiplier = Mathf.Clamp01(Mathf.Abs(rb.velocity.y) / 20f);
                SFXManager.Instance.PlayPlayerLandSound(volumeMultiplier);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ActivateRespawnEffects();
            }

            
           
        }
    }
 
    public void RespawnPlayer()
    {
        rb.MovePosition(startPosition);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionZ;

        isJumping = false;

        // Play respawn sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayEffect("Respawn", 1f);
        }

        if (AdaptiveMusicController.Instance != null)
        {
            AdaptiveMusicController.Instance.OnPlayerDeath();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("SlideGround"))
        {
            smoothTime = 1f;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            if (!canFreeze && !isWallJumping && !isJumping && !isTouchingWall)
            {
                freezeTimerCounter++;
                if (freezeTimerCounter >= freezeTimer)
                {
                    canFreeze = true;
                    freezeTimerCounter = 0;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("SlideGround"))
        {
            smoothTime = 0.3f;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            canFreeze = false;
            freezeTimerCounter = 0;
            animator.SetBool("isWallHanging", false);
        }

        if (collision.collider.CompareTag("Wall"))
        {
            animator.SetBool("isWallHanging", false);
        }
    }
}

