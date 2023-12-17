using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Variáveis públicas
    public float moveForce = 35f;
    public float maxSpeed = 15f;
    public float baseJumpVelocity = 14f;
    public float jumpSpeedMultiplier = 0.4f;
    public float fallMultiplier = 6f;
    public float lowJumpMultiplier = 3f;
    public float brakeForce = 10f;
    public AudioSource jumpSound;
    public AudioSource bounceSound;
    public AudioSource fallingSound;

    // Variáveis privadas
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private bool canMoveHorizontally = false;
    private bool canJump = false;
    private float jumpBufferTime = 0.2f;
    private float timeSinceJumpPressed;
    private bool jumpBuffered;
    private int facingDirection = -1;
    private float runSpeedThreshold = 10f;
    private float sprintSpeedThreshold = 14f;

    private float wallCollisionDisableTime = 0.1f;

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
        HandleScore();
    }

    private void SubscribeToEvents()
    {
        GameEvents.OnEndGame += HandleDeath;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.OnEndGame -= HandleDeath;
    }

    private void HandleScore()
    {
        int height = (gameObject.transform.position.y).ConvertTo<int>();

        if (height > GameManager.Instance.Score)
        {
            GameManager.Instance.Score = height;
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

            UpdateAnimationBasedOnSpeed(currentSpeed);
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

    private void UpdateAnimationBasedOnSpeed(float speed)
    {
        if (isJumping) return;

        animator.ResetTrigger("Idle_01");

        if (speed > 0 && speed < runSpeedThreshold)
        {
            animator.SetTrigger("Walk_01");
        }
        else if (speed >= runSpeedThreshold && speed < sprintSpeedThreshold)
        {
            animator.SetTrigger("Run_01");
        }
        else if (speed >= sprintSpeedThreshold)
        {
            animator.SetTrigger("Run_02");
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

        if (
            isGrounded &&
            jumpBuffered &&
            !isJumping &&
            canJump
            )
        {
            PerformJump();
            jumpBuffered = false;
        }
    }

    private void PerformJump()
    {
        isGrounded = false;
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

        PlayJumpSound();
    }

    private void PlayJumpSound()
    {
        if (jumpSound.isPlaying)
        {
            jumpSound.Stop();
        }
        else
        {
            jumpSound.Play();
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
        if (collision.gameObject.CompareTag("Platform"))
        {
            canMoveHorizontally = true;
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            PlayBounceSound();
            StartCoroutine(DisableMovementTemporarily());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
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
        Debug.Log("Attempt to PlayFallingSound");
        if (fallingSound.isPlaying)
        {
            fallingSound.Stop();
        }
        else
        {
            Debug.Log("Attempt to PlayFallingSound");
            fallingSound.Play();
        }
    }
}
