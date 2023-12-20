using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Variáveis públicas
    public float moveForce = 20f;
    public float maxSpeed = 15f;
    public float baseJumpVelocity = 11f;
    public float jumpSpeedMultiplier = 0.4f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 4f;
    public float brakeForce = 10f;

    public ParticleSystem fastJumpingVfx;

    //Audios
    public AudioSource jumpSound;
    public AudioSource bounceSound;
    public AudioSource fallingSound;
    public AudioSource yeahSound;

    // Variáveis privadas
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool canMoveHorizontally = false;
    private bool canJump = true;
    private float jumpBufferTime = 0.2f;
    private float timeSinceJumpPressed;
    private bool jumpBuffered;
    private int facingDirection = -1;
    private float runSpeedThreshold = 10f;
    private float sprintSpeedThreshold = 14f;

    private int runningSpeedLevel = 1;

    private float wallCollisionDisableTime = 0.1f;

    private int previousPlatformFloorLevel;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void FixedUpdate()
    {
        HandleHorizontalMovement();
        HandleJumpInput();
        UpdateJumpBuffer();
        ApplyGravity();
        CheckForFall();
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnEndGame += HandleDeath;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnEndGame -= HandleDeath;
    }

    private void HandleScore(int currentFloor)
    {
        GameManager.Instance.Floor = currentFloor;

        if (currentFloor > previousPlatformFloorLevel++)
        {
            GameManager.Instance.ComboCounter += currentFloor - previousPlatformFloorLevel;
        }

        if (currentFloor <= previousPlatformFloorLevel)
        {
            GameManager.Instance.ComboCounter = 0;
        }
    }

    private void HandleHorizontalMovement()
    {
        if (!canMoveHorizontally) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed = Mathf.Abs(rb.velocity.x);

        if (Mathf.Abs(moveInput) > 0)
        {
            if ((moveInput > 0 && facingDirection < 0) || (moveInput < 0 && facingDirection > 0))
            {
                FlipCharacter();
            }

            if (moveInput * rb.velocity.x < 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if (currentSpeed < maxSpeed)
            {
                rb.AddForce(new Vector2(moveInput * moveForce, 0));
            }

            UpdateAnimationAndSpeedLevel(currentSpeed);
        }
        else
        {
            ApplyBraking();

            if (!isJumping)
            {
                animator.SetTrigger("Idle_01");
            }
        }
    }

    private void UpdateAnimationAndSpeedLevel(float speed)
    {
        if (isJumping) return;

        animator.ResetTrigger("Idle_01");

        if (speed > 0 && speed < runSpeedThreshold)
        {
            runningSpeedLevel = 1;
            animator.SetTrigger("Walk_01");
        }
        else if (speed >= runSpeedThreshold && speed < sprintSpeedThreshold)
        {
            runningSpeedLevel = 2;
            animator.SetTrigger("Run_01");
        }
        else if (speed >= sprintSpeedThreshold)
        {
            runningSpeedLevel = 3;
            animator.SetTrigger("Run_02");
        }
        else
        {
            runningSpeedLevel = 0;
        }
    }

    private void ApplyBraking()
    {
        if (canMoveHorizontally && rb.velocity.x != 0)
        {
            float brakingForce = Mathf.Sign(rb.velocity.x) * -brakeForce;
            rb.AddForce(new Vector2(brakingForce, 0));

            if (Mathf.Abs(rb.velocity.x) < 1f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private void FlipCharacter()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void HandleJumpInput()
    {
        if (Input.GetKey(KeyCode.Space) || jumpBuffered)
        {
            if (isGrounded && !isJumping)
            {
                PerformJump();
                jumpBuffered = false;
            }
            else if (!isGrounded && !jumpBuffered)
            {
                jumpBuffered = true;
                timeSinceJumpPressed = 0f;
            }
        }
    }

    private void UpdateJumpBuffer()
    {
        if (jumpBuffered)
        {
            timeSinceJumpPressed += Time.deltaTime;
            if (timeSinceJumpPressed > jumpBufferTime)
            {
                jumpBuffered = false;
            }
        }
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void CheckForFall()
    {
        if (rb.velocity.y < 0 && isJumping)
        {
            isJumping = false;
        }

        if (isGrounded && jumpBuffered && !isJumping)
        {
            PerformJump();
            jumpBuffered = false;
        }
    }

    private void PerformJump()
    {
        if (!canJump) return;

        isJumping = true;
        jumpBuffered = false;

        float extraJumpVelocity = Mathf.Abs(rb.velocity.x) * jumpSpeedMultiplier;
        rb.velocity = new Vector2(rb.velocity.x, baseJumpVelocity + extraJumpVelocity);

        // ToDo Check improvement to avoid running all those ResetTriggers methods
        animator.ResetTrigger("Idle_01");
        animator.ResetTrigger("Walk_01");
        animator.ResetTrigger("Run_01");
        animator.ResetTrigger("Run_02");
        animator.ResetTrigger("Still_01");
        animator.SetTrigger("Jump_01");

        PlayJumpSoundAndAnimation();
    }

    private void PlayJumpSoundAndAnimation()
    {
        if (jumpSound.isPlaying)
        {
            jumpSound.Stop();
            jumpSound.Play();
        }
        else
        {
            jumpSound.Play();
        }


        //ToDo change audioSource for level3 sound (not yet implemented)

        if (runningSpeedLevel >= 2)
        {
            if (fastJumpingVfx != null)
            {
                ResetParticleSystem();
            }

            if (yeahSound.isPlaying)
            {
                yeahSound.Stop();
                yeahSound.Play();
            }
            else
            {
                yeahSound.Play();
            }
        }
    }

    private void PlayBounceSound()
    {
        if (bounceSound.isPlaying)
        {
            bounceSound.Stop();
        }
        else
        {
            bounceSound.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Platform") && rb.velocity.y <= 0)
        {


            canMoveHorizontally = true;
            isGrounded = true;

            if (fastJumpingVfx != null)
            {
                ResetParticleSystem(shouldStop: true);
            }

            HandleScore(collision.gameObject.GetComponent<PlatformData>().floorLevel);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            PlayBounceSound();
            StartCoroutine(DisableMovementTemporarily());
        }
    }

    private void ResetParticleSystem(bool shouldStop = false)
    {

        if (fastJumpingVfx.isPlaying)
            fastJumpingVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (!shouldStop)
            fastJumpingVfx.Play();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (isGrounded)
            {
                previousPlatformFloorLevel = collision.gameObject.GetComponent<PlatformData>().floorLevel;
            }

            isGrounded = false;
        }
    }

    private IEnumerator DisableMovementTemporarily()
    {
        canMoveHorizontally = false;
        yield return new WaitForSeconds(wallCollisionDisableTime);
        canMoveHorizontally = true;
    }

    private void HandleDeath()
    {
        canMoveHorizontally = false;
        canJump = false;
        PlayFallingSound();
        StartCoroutine(LoadGameOver());
    }

    private IEnumerator LoadGameOver()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOver");
    }

    private void PlayFallingSound()
    {
        if (fallingSound.isPlaying)
        {
            fallingSound.Stop();
        }
        else
        {
            fallingSound.Play();
        }
    }
}
