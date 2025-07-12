using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 4;
    private Rigidbody2D rb;
    private Vector2 inputVector;

    void FixedUpdate()
    {
        rb.linearVelocity = speed * inputVector;
    }


    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }
}
