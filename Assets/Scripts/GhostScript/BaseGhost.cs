using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseGhost : MonoBehaviour
{
    public Transform target;
    public GameObject Bullet;
    [HideInInspector] bool isDying;
    [HideInInspector] bool HardGhost;
    [HideInInspector] bool Alive;
    [HideInInspector] public float aggroRange = 8f;
    [HideInInspector] public LayerMask lineOfSightMask;
    [HideInInspector] public bool requireLineOfSight = false;

    [HideInInspector] public float Speed = 3f;
    [HideInInspector] public float WanderSpeed = 1.5f;
    [HideInInspector] public float Amplitude = 0.1f;
    [HideInInspector] public float Frequency = 2f;
    [HideInInspector] public float Acceleration = 5f;
    [HideInInspector] public float TurningSpeed = 3f;
    [HideInInspector] public float invulnerabilityDuration = 1.5f;

    [HideInInspector] private float StruggleAmplitude = 200f;
    [HideInInspector] private float StruggleSpeed = 15f;

    protected Vector2 velocityPosition;
    protected float floatTimer;
    protected float lastDamageTime = -Mathf.Infinity;
    protected bool isAggroed = false;

    protected Rigidbody2D rb;
    protected Vector2 externalForce = Vector2.zero;
    protected Vector2 currentVelocity = Vector2.zero;

    private Vector2 wanderDirection = Vector2.zero;
    private float wanderTimer = 0f;
    private float wanderInterval = 3f;
    private float struggleTimer = 0f;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isPulling;
    [HideInInspector] public string GhostType = "Contact";

    protected virtual void Start()
    {
        HardGhost = true;
        Alive = true;
        isPulling = false;
        rb = GetComponent<Rigidbody2D>();
        velocityPosition = rb.position;

        Collider2D myCollider = GetComponent<Collider2D>();
        foreach (Collider2D col in FindObjectsOfType<Collider2D>())
        {
            if (col != myCollider && col.CompareTag("Ghost"))
            {
                Physics2D.IgnoreCollision(myCollider, col);
            }
        }

        SetRandomWanderDirection();
    }

    protected virtual void Update()
    {
        if (!Alive)
        {
            isDying = true;
            Destroy(gameObject, 3f);
            return;
        }
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        isAggroed = distanceToTarget <= aggroRange && (!requireLineOfSight || HasLineOfSight());

        if (isAggroed)
        {
            MoveToTarget();
        }
        else
        {
            Wander();
        }
    }

    protected void MoveToTarget()
    {
        if (!Alive) return;

        Vector2 toTarget = ((Vector2)target.position - velocityPosition).normalized;

        if (isPulling)
        {
            struggleTimer += Time.deltaTime * StruggleSpeed;
            Vector2 perpendicular = new Vector2(-toTarget.y, toTarget.x);
            float sway = Mathf.Sin(struggleTimer) * StruggleAmplitude;
            Vector2 struggleDir = (-toTarget * 5f + perpendicular * sway).normalized;
            currentVelocity = Vector2.Lerp(currentVelocity, struggleDir * Speed, TurningSpeed * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, toTarget * Speed, TurningSpeed * Time.deltaTime);
        }

        MoveWithFloat();
    }

    protected void Wander()
    {
        if (!Alive) return;

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            SetRandomWanderDirection();
        }

        currentVelocity = Vector2.Lerp(currentVelocity, wanderDirection * WanderSpeed, TurningSpeed * Time.deltaTime);
        MoveWithFloat();
    }

    private void SetRandomWanderDirection()
    {
        if (!Alive) return;

        float angle = UnityEngine.Random.Range(0f, 360f);
        wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        wanderTimer = UnityEngine.Random.Range(2f, 5f);
    }

    private void MoveWithFloat()
    {
        if (!Alive) return;

        floatTimer += Time.deltaTime;
        float floatOffset = Mathf.Cos(floatTimer * Frequency) * Amplitude;
        Vector2 movement = currentVelocity * Time.deltaTime + externalForce * Time.deltaTime;
        velocityPosition += movement;
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
        Vector2 finalPosition = velocityPosition + new Vector2(0f, floatOffset);
        rb.MovePosition(finalPosition);
    }

    protected bool HasLineOfSight()
    {
        if (!Alive) return false;

        Vector2 direction = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, aggroRange, lineOfSightMask);
        return hit.collider != null && hit.collider.transform == target;
    }

    public virtual void ApplyExternalForce(Vector2 force)
    {
        if (!Alive) return;

        externalForce += force;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!Alive) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(1);
            isAttacking = true;
            lastDamageTime = Time.time;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet")&& !HardGhost)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            Alive = false;
        }
        if (other.gameObject.CompareTag("Bullet") && HardGhost)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            Alive = false;
            Instantiate(Bullet, transform.position, transform.rotation);
        }
    }

    public void ApplySlow(float amount, float duration)
    {
        if (!Alive) return;

        StartCoroutine(SlowDown(amount, duration));
    }

    private IEnumerator SlowDown(float amount, float duration)
    {
        Speed -= amount;
        yield return new WaitForSeconds(duration);
        Speed += amount;
    }

    public void teleport(Vector2 newPos)
    {
        if (!Alive) return;

        rb.position = newPos;
        velocityPosition = newPos;
        externalForce = Vector2.zero;
        floatTimer = 0f;
    }

    public void GotHit(Action action)
    {
        if (!Alive) return;

        action?.Invoke();
    }
}
