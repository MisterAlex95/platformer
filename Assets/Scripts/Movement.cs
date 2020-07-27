﻿using UnityEngine;

public class Movement : MonoBehaviour {
    public float moveSpeed;
    public float climbSpeed;
    public float jumpForce;

    [HideInInspector]
    public bool isJumping = false;
    [HideInInspector]
    public bool isGrounded = false;
    [HideInInspector]
    public bool isClimbing = false;
    
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask collisionLayers;

    public static Movement instance;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public CapsuleCollider2D playerCollider;
    
    private Vector3 velocity = Vector3.zero;
    private float horizontalMovement;
    private float verticalMovement;

    private void Awake() {
        if (instance) {
            Debug.LogWarning("There is more than one instance of Movement !");
            return;
        }
        instance = this;
    }

    // Only for physics
    void FixedUpdate() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);
        MovePlayer(horizontalMovement, verticalMovement);
    }

    void Update() {
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
        verticalMovement = Input.GetAxis("Vertical") * climbSpeed * Time.fixedDeltaTime;

        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing) {
            isJumping = true;
        }

        Flip(rb.velocity.x);

        float characterVelocity = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("speed", characterVelocity);
        animator.SetBool("isClimbing", isClimbing);
    }

    void MovePlayer(float _horizontalMovement, float _verticalMovement) {
        if (!isClimbing) {
            Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

            if (isJumping) {
                rb.AddForce(new Vector2(0f, jumpForce));
                isJumping = false;
            }
        } else {
            Vector3 targetVelocity = new Vector2(0, _verticalMovement);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        }
    }

    void Flip(float _velocity) {
        if (_velocity > 0.1f) {
            spriteRenderer.flipX = false;
        } else if (_velocity < -0.1f) {
            spriteRenderer.flipX = true;
        }
    } 

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}