using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class HomelessBullet : MonoBehaviour
{
    public GameObject electricField;
    public Vector3 targetSize = new Vector3(4f, 4f, 4f);
    private float startTime;
    private float timeToLive = 5f;
    [HideInInspector] public Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime >= timeToLive)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ghost") || collision.gameObject.CompareTag("Angel"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();

            Rigidbody2D _rb = GetComponent<Rigidbody2D>();

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 10;

            Debug.Log(_rb.linearVelocity.normalized);

            controller.ApplyExternalForce(transform.right * 30);

            Instantiate(electricField, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossHandler>().TakeDamege(7);
        }
        Destroy(gameObject);

    }

}
