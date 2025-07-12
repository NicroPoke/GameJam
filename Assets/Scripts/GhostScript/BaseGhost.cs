using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseGhost : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    [HideInInspector] public float Speed = 5f;
    [HideInInspector] public float Amplitude = 0.1f;
    [HideInInspector] public float Frequency = 2f;

    [Header("Damage Settings")]
    [HideInInspector] public float invulnerabilityDuration = 1.5f;

    protected Vector2 velocityPosition;
    protected float floatTimer;
    protected float lastDamageTime = -Mathf.Infinity;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isPulling;
    [HideInInspector] public string GhostType = "Contact";

    protected Rigidbody2D rb;
    protected Vector2 externalForce = Vector2.zero;

    protected virtual void Start()
    {
        isPulling = false;
        rb = GetComponent<Rigidbody2D>();
        velocityPosition = rb.position;
    }

    protected virtual void Update()
    {
        if (target == null) return;

        MoveGhost();
    }

    protected virtual void MoveGhost()
    {
        Vector2 direction = ((Vector2)target.position - velocityPosition).normalized;
        floatTimer += Time.deltaTime;
        float floatOffset = Mathf.Cos(floatTimer * Frequency) * Amplitude;
        Vector2 movement = (isPulling ? -direction : direction) * Speed * Time.deltaTime;
        velocityPosition += movement + externalForce * Time.deltaTime;
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);
        rb.MovePosition(velocityPosition + new Vector2(0f, floatOffset));
    }

    public virtual void ApplyExternalForce(Vector2 force)
    {
        externalForce += force;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(1);
            isAttacking = true;
            Debug.Log("Contact with player.");
            lastDamageTime = Time.time;
        }
    }
}