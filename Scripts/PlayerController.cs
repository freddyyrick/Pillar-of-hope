using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float runSpeed = 10f;
    public float airWalkSpeed = 25f;
    public float jumpImpulse = 12f;

    [Header("Better Jump Settings")]
    public float fallMultiplier = 5.5f;
    public float lowJumpMultiplier = 5.5f;

    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
    [SerializeField] private bool _isFacingRight = true;
    private Rigidbody2D rb;
    private Animator animator;

    private bool jumpHeld;

    // Added missing field
    private bool IsMoving;

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (value ? 1 : -1);
                transform.localScale = scale;
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }
    public float CurrentMoveSpeed
    {
        get
        {
            if (!CanMove) return 0f;
            if (moveInput.x != 0 && touchingDirections.IsOnWall) return 0f;

            return touchingDirections.IsGrounded ? runSpeed : airWalkSpeed;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    
    }

    private void FixedUpdate()
    {
        if(!damageable.LockVelocity)
        rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        //handling running animation
        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > 0.01f);

        // Better jump
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpHeld)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight) IsFacingRight = true;
        else if (moveInput.x < 0 && IsFacingRight) IsFacingRight = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }

        if (context.performed) jumpHeld = true;
        if (context.canceled) jumpHeld = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }
    public void OnHit(int damage, Vector2 knockback)
    {
        LockVelocity = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y * knockback.y);
    }
}
