using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Knight : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float walkStopRate = 0.6f;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkableDirection { Right, Left }
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                transform.localScale = new Vector2(
                    transform.localScale.x * -1,
                    transform.localScale.y
                );

                if (value == WalkableDirection.Right)
                    walkDirectionVector = Vector2.right;
                else if (value == WalkableDirection.Left)
                    walkDirectionVector = Vector2.left;
            }

            _walkDirection = value;
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void Update()
    {

    // Remove dead or destroyed targets from the detection list
    attackZone.detectedColliders.RemoveAll(collider =>
    {
        if (collider == null) return true; // collider destroyed (player object removed)
        
        Damageable d = collider.GetComponent<Damageable>();
        // Remove collider if it has a Damageable and is no longer alive
        return d != null && !d.IsAlive;
    });

    // If there’s any valid, living target — set HasTarget = true
    HasTarget = attackZone.detectedColliders.Count > 0;


    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        } 
        //Move only if not locked
        if (!damageable.LockVelocity)
        {
            if (CanMove)
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
            WalkDirection = WalkableDirection.Left;
        else if (WalkDirection == WalkableDirection.Left)
            WalkDirection = WalkableDirection.Right;
        else
            Debug.LogError("Current WalkDirection is not set to legal values of right or left.");
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if(touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
}
