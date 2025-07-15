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
    [HideInInspector] public int health = 100;
    [SerializeField] private float pullForceMultiplier = 5f;
    [SerializeField] public float speed = 9f;
    private Rigidbody2D rb;
    private Vector2 inputVector;
    private BaseGhost lastGhost;

    void FixedUpdate()
    {
        rb.linearVelocity = speed * inputVector;
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
        GameObject target = GameObject.Find("SliderHP");

        if (target != null)
        {
            target.GetComponent<SliderScript>().ChangeSliderValue(health);
        }
    }
}
