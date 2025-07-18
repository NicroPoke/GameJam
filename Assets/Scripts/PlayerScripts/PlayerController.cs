using NUnit.Framework;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    bool isPullerActive = false;
    public Animator animator;
    [HideInInspector] public int health = 100;
    [SerializeField] private float pullForceMultiplier = 5f;
    public float baseSpeed = 9f;
    public float speed;
    private Rigidbody2D rb;
    [HideInInspector] public bool IsWalking = false;
    [HideInInspector] public Vector2 inputVector;
    public float currentVelocity = 0f;
    private BaseGhost lastGhost;

    void FixedUpdate()
    {
        animator.SetBool("IsWalking", IsWalking);
        rb.linearVelocity = speed * inputVector;
        currentVelocity = rb.linearVelocity.sqrMagnitude;
        if (currentVelocity > 0)
        {
            IsWalking = true;
        }
        else
        {
            IsWalking = false;
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        lastGhost = null;

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

        ChangeSliderHP();
    }

    void ChangeSliderHP()
    {
        GameObject target = GameObject.Find("HP");

        if (target != null)
        {
            target.GetComponent<UnityEngine.UI.Image>().fillAmount = health * 0.01f;
        }
    }
}
