using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    private int health = 100;
    [SerializeField] private float pullForceMultiplier = 5f;
    [SerializeField] public float speed = 4f;
    private Rigidbody2D rb;
    private Vector2 inputVector;

    void FixedUpdate()
    {
        rb.linearVelocity = speed * inputVector;
    }

    void Update()
    {
        if (health <= 0) {
            Die();
        }
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    void OnPull(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(input);
        mouseToWorld.z = 0;
        Vector2 direction = ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;

        float distance = Vector2.Distance(transform.position, mouseToWorld);

        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(transform.position, direction, distance);

        foreach (var hit in raycastHits) {
            if (hit.collider.gameObject.CompareTag("Ghost"))
            {
                BaseGhost ghost = hit.collider.gameObject.GetComponent<BaseGhost>();
                if (ghost != null)
                {
                    ghost.ApplyExternalForce(-direction * pullForceMultiplier);
                }
            }
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Game Over");
    }

    public void TakeDamege(int damage)
    {
        health -= damage;
    }
}
