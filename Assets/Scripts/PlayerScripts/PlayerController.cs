using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private int health = 100;
    [SerializeField] private float speed = 4;
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
