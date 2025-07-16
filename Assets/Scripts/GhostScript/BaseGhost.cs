using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseGhost : MonoBehaviour
{
    public LayerMask wall;
    private Animator animator;
    [HideInInspector] public Transform target;
    public GameObject Bullet;
    [HideInInspector] public bool isDying;
    [HideInInspector] public bool HardGhost;
    [HideInInspector] public bool Alive;
    [HideInInspector] public float aggroRange = 8f;
    [HideInInspector] public LayerMask lineOfSightMask;
    [HideInInspector] public bool requireLineOfSight = false;
    [HideInInspector] public float Speed = 2f;
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

    bool factingRight = false;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        HardGhost = true;
        Alive = true;
        isPulling = false;
        rb = GetComponent<Rigidbody2D>();
        velocityPosition = rb.position;
        HardGhost = false;

        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

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
        AnimationCorrector();
        if (!Alive)
        {
            Die();
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
        ChangeDirection();
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

    protected virtual void MoveWithFloat()
    {
        if (!Alive)
        {
            Vector2 movement = externalForce * Time.deltaTime;
            velocityPosition += movement;
            externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
            rb.MovePosition(velocityPosition);
        }
        else
        {
            floatTimer += Time.deltaTime;
            float floatOffset = Mathf.Cos(floatTimer * Frequency) * Amplitude;
            Vector2 movement = currentVelocity * Time.deltaTime + externalForce * Time.deltaTime;
            velocityPosition += movement;
            externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
            Vector2 finalPosition = velocityPosition + new Vector2(0f, floatOffset);
            rb.MovePosition(finalPosition);
        }
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

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!Alive) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(1);
            isAttacking = true;
            lastDamageTime = Time.time;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) isAttacking = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet") && !HardGhost)
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
    protected virtual void AnimationCorrector()
    {
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isPulling", isPulling);
        animator.SetBool("isDiyng", isDying);
    }
    public void GotHit(Action action)
    {
        if (!Alive) return;

        action?.Invoke();
    }

    protected virtual void Die()
    {
        isDying = true;
        Invoke(nameof(PlayPopSound), 0f);
        Destroy(gameObject, 0.3f);
    }

    void PlayPopSound()
    {
        GameObject soundObject = GameObject.FindWithTag("Sound");
        if (soundObject != null)
        {
            Sound sound = soundObject.GetComponent<Sound>();
            if (sound != null && sound.pop != null)
            {
                sound.pop.Stop(); 
                sound.pop.Play();
            }
            else
            {
                Debug.LogWarning("Pop sound или компонент Sound не назначены.");
            }
        }
        else
        {
            Debug.LogWarning("Не найден объект с тегом 'Sound'.");
        }
    }

    public void PlayPopThenDestroy()
    {
        PlayPopSound();
        Destroy(gameObject);
    }

    void ChangeDirection()
    {
        Vector2 direction = new Vector2(0, 0);
        if (isPulling)
            direction = currentVelocity;
        else
            direction = -currentVelocity;

        float angle = Mathf.Atan2(direction.y, direction.x);
        float cos = math.cos(angle);

        if (cos > 0 && factingRight)
        {
            FlipCharacter();
            factingRight = false;
        }
        else if (cos < 0 && !factingRight)
        {
            FlipCharacter();
            factingRight = true;
        }
    }

    void FlipCharacter()
    {
        Vector3 euler = transform.eulerAngles;
        euler.y = euler.y + 180f;
        transform.eulerAngles = euler;
    }
}
