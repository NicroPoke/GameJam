using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseGhost : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    [HideInInspector] public float Speed = 3f;
    [HideInInspector] public float Amplitude = 0.1f;
    [HideInInspector] public float Frequency = 2f;
    [HideInInspector] public float Acceleration = 5f;
    [HideInInspector] public float MaxSpeed = 3.5f;
    [HideInInspector] public float TurningSpeed = 3f;

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
    protected Vector2 currentVelocity = Vector2.zero;

    protected virtual void Start()
    {
        isPulling = false;
        rb = GetComponent<Rigidbody2D>();
        velocityPosition = rb.position;

        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col != myCollider && col.CompareTag("Ghost"))
            {
                Physics2D.IgnoreCollision(myCollider, col);
            }
        }
    }

    protected virtual void Update()
    {
        if (target == null) return;
        MoveGhost();
    }

    protected virtual void MoveGhost()
    {
        Vector2 toTarget = ((Vector2)target.position - velocityPosition);
        Vector2 desiredDirection = toTarget.normalized * (isPulling ? -1f : 1f);

        currentVelocity = Vector2.MoveTowards(currentVelocity, desiredDirection * MaxSpeed, Acceleration * Time.deltaTime);

        floatTimer += Time.deltaTime;
        float floatOffset = Mathf.Cos(floatTimer * Frequency) * Amplitude;

        Vector2 movement = currentVelocity * Time.deltaTime + externalForce * Time.deltaTime;
        velocityPosition += movement;
        externalForce = Vector2.Lerp(externalForce, Vector2.zero, Time.deltaTime * 5f);

        Vector2 finalPosition = velocityPosition + new Vector2(0f, floatOffset);
        rb.MovePosition(finalPosition);
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
            lastDamageTime = Time.time;
        }
    }

    public void ApplySlow(float amount, float duration)
    {
        StartCoroutine(SlowDown(amount, duration));
    }

    System.Collections.IEnumerator SlowDown(float amount, float duration)
    {
        Speed -= amount;
        yield return new WaitForSeconds(duration);
        Speed += amount;
    }

    public void teleport(Vector2 newPos)
    {
        rb.position = newPos;
        velocityPosition = newPos;
        externalForce = Vector2.zero;
        floatTimer = 0f;
    }

    public void GotHit(Action action)
    {
        action?.Invoke();
    }
}
