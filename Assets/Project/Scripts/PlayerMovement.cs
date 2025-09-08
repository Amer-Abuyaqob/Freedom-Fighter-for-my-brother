using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sp;
    [SerializeField] Animator animator;
    [SerializeField] PlayerCombat combat;
    [SerializeField] float speed = 1.35f;
    [SerializeField] float jumpPower = 5f;
    [SerializeField] string groundTag = "Ground";

    float horizontalInput = 0f;
    bool jumpRequested = false;
    bool canJump = false;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sp == null) sp = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
        if (combat == null) combat = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        // If attacking, lock input
        if (combat != null && combat.IsAttacking)
        {
            horizontalInput = 0f;
            jumpRequested = false;
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        // Read horizontal input (-1, 0, 1)
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) horizontalInput -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) horizontalInput += 1f;

        // Jump input (edge-triggered)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            jumpRequested = true;

        // Animator: walking state
        if (animator != null)
            animator.SetBool("isWalking", Mathf.Abs(horizontalInput) > 0.01f);
    }

    void FixedUpdate()
    {
        // If attacking, stop horizontal movement
        if (combat != null && combat.IsAttacking)
        {
            if (rb != null)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            return;
        }

        // Horizontal movement handled via velocity in physics step
        if (rb != null)
        {
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        }

        // Sprite facing
        // FIXME: Use scale.x * -1 method instead (so all of the children of the gameObject gets mirrored)
        if (sp != null)
        {
            if (horizontalInput > 0.01f) sp.flipX = false;
            else if (horizontalInput < -0.01f) sp.flipX = true;
        }

        // Jump
        if (jumpRequested && canJump && rb != null)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        }

        // Clear one-shot input
        jumpRequested = false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == groundTag)
            canJump = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == groundTag)
            canJump = false;
    }
}
