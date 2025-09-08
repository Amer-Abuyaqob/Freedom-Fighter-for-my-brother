using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform patrolLeft;
    [SerializeField] Transform patrolRight;
    [SerializeField] Transform attackPoint; // optional, for gizmos
    [SerializeField] MeleeHitbox meleeHitbox; // child hitbox

    [Header("Behavior Settings")]
    [SerializeField] float patrolSpeed = 1.2f;
    [SerializeField] float chaseSpeed = 1.8f;
    [SerializeField] float chaseRange = 2.2f;
    [SerializeField] float attackRange = 0.9f;
    [SerializeField] float attackCooldown = 0.9f;
    [SerializeField] bool invertFacing = false;
        

    [Header("Animation Parameters")] 
    [SerializeField] string isWalkingParam = "isWalking";
    [SerializeField] string isChasingParam = "isChasing";
    [SerializeField] string inAttackRangeParam = "inAttackRange";
    [SerializeField] string attackTrigger = "Attack";

    Transform player;
    float nextAttackTime = 0f;
    int direction = 1; // 1 right, -1 left for patrol
    float patrolMinX;
    float patrolMaxX;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Try to auto-find children by name if not assigned
        if (patrolLeft == null)
        {
            var t = transform.Find("PatrolLeft");
            if (t != null) patrolLeft = t;
        }
        if (patrolRight == null)
        {
            var t = transform.Find("PatrolRight");
            if (t != null) patrolRight = t;
        }
        if (meleeHitbox == null)
        {
            meleeHitbox = GetComponentInChildren<MeleeHitbox>(true);
        }

        // Capture patrol bounds in world space so child markers moving with the enemy won't affect logic
        if (patrolLeft != null && patrolRight != null)
        {
            patrolMinX = Mathf.Min(patrolLeft.position.x, patrolRight.position.x);
            patrolMaxX = Mathf.Max(patrolLeft.position.x, patrolRight.position.x);
        }
        else
        {
            // Fallback: small roaming range around start position
            patrolMinX = transform.position.x - 1f;
            patrolMaxX = transform.position.x + 1f;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool shouldChase = distanceToPlayer <= chaseRange && distanceToPlayer > attackRange;
        bool inAttackRange = distanceToPlayer <= attackRange;

        if (inAttackRange)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            UpdateAnimator(false, true, true);
            TryAttack();
        }
        else if (shouldChase)
        {
            Chase();
            UpdateAnimator(true, true, false);
        }
        else
        {
            Patrol();
            UpdateAnimator(Mathf.Abs(rb.velocity.x) > 0.01f, false, false);
        }
    }

    void Patrol()
    {
        // Reverse at captured world-space patrol bounds
        if (direction > 0 && transform.position.x >= patrolMaxX)
            direction = -1;
        else if (direction < 0 && transform.position.x <= patrolMinX)
            direction = 1;

        rb.velocity = new Vector2(direction * patrolSpeed, rb.velocity.y);
        ApplyFacing(direction);
    }

    void Chase()
    {
        if (player == null) return;
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
        ApplyFacing(dir);
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
    }

    void ApplyFacing(float dir)
    {
        if (invertFacing) dir = -dir;
        float absX = Mathf.Abs(transform.localScale.x);
        if (dir > 0.01f) transform.localScale = new Vector3(absX, transform.localScale.y, transform.localScale.z);
        else if (dir < -0.01f) transform.localScale = new Vector3(-absX, transform.localScale.y, transform.localScale.z);
    }

    void UpdateAnimator(bool isWalking, bool isChasing, bool inAttack)
    {
        if (animator == null) return;
        if (!string.IsNullOrEmpty(isWalkingParam)) animator.SetBool(isWalkingParam, isWalking);
        if (!string.IsNullOrEmpty(isChasingParam)) animator.SetBool(isChasingParam, isChasing);
        if (!string.IsNullOrEmpty(inAttackRangeParam)) animator.SetBool(inAttackRangeParam, inAttack);
    }

    // Animation Events (call these in attack clips)
    public void OnMeleeHitboxEnable()
    {
        if (meleeHitbox != null) meleeHitbox.EnableHitbox();
    }

    public void OnMeleeHitboxDisable()
    {
        if (meleeHitbox != null) meleeHitbox.DisableHitbox();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint != null ? attackPoint.position : transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(patrolMinX, transform.position.y, 0), new Vector3(patrolMaxX, transform.position.y, 0));
    }
}


