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
    [SerializeField] private bool isGrounded;
    private bool jump = false;
    private bool isJumping = true;

    [Header("Player Wall Jump")]
    [SerializeField] private float wallJumpForce;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector3 boxSizeWall;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
       // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (rb.velocity.y < 0)
        {
            animator.SetBool("isJumping", false);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            animator.SetBool("isRunning", true);
            currentMovSpeed = movSpeedRunning;
          
        }
        else
        {
            animator.SetBool("isRunning", false);
            currentMovSpeed = movSpeed;
     
        }

        movHorizontal = Input.GetAxisRaw("Horizontal") * currentMovSpeed;

        if (movHorizontal != 0 && !Input.GetKey(KeyCode.Z))
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }


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

        isGrounded = Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, groundLayer);
        animator.SetBool("isGrounded", isGrounded);
        isTouchingWall = Physics.CheckBox(wallCheck.position, boxSizeWall, Quaternion.identity, wallLayer);

        bool isWallHanging = isTouchingWall && !isGrounded;
        animator.SetBool("isWallHanging", isWallHanging);

        if(isWallHanging)
        {
            animator.SetBool("isJumping", false);
        }
    
        //if (isGrounded)
        //{

        //    if (!canFreeze)
        //    {
        //        freezeTimerCounter++;
        //        if (freezeTimerCounter >= freezeTimer)
        //        {
        //            canFreeze = true;
        //            freezeTimerCounter = 0;
        //        }
        //    }
        //}

        if (isGrounded && !isJumping && canFreeze)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        }

    }
    
    private void FixedUpdate()
    {
        
        

        MovePlayer(movHorizontal * Time.fixedDeltaTime, jump, wallJump);

        

        jump = false;
        wallJump = false;
    }

    private void MovePlayer(float move, bool jumping, bool wallJumping)
    {
        Vector3 targetVelocity = new Vector3(movHorizontal, rb.velocity.y, 0);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothTime);

        if (movHorizontal > 0 && !isLookingRight)
        {
            Flip();
        }
        else if (movHorizontal < 0 && isLookingRight)
        {
            Flip();
        }

        if (wallJumping && isTouchingWall && !isGrounded && canWallJump && !isWallJumping)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isTouchingWall = false;
            canWallJump = false;
            isWallJumping = true;
            animator.SetBool("isWallJumping", true);
            
        }

        if (jumping && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            animator.SetBool("isJumping", true);
            animator.SetBool("isWallJumping", false);
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
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheck.position, boxSizeWall);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            RespawnPlayer();
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            RespawnPlayer();
        }
    }
 
    public void RespawnPlayer()
    {
        transform.position = startPosition;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

        isJumping = false;

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
    }
}

