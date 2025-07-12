using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class ChildBullet : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    private float timeToBeTriggered = 3f;


    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;

        startTime = Time.time;

    }

    void Update()
    {
        if (Time.time - startTime >= timeToBeTriggered)
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
        }

        if (Time.time - startTime >= timeToLive)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ghost"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();

            Rigidbody2D _rb = GetComponent<Rigidbody2D>();

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 10;

            Debug.Log(_rb.linearVelocity.normalized);

            controller.ApplyExternalForce(transform.right * 30);

            Destroy(gameObject);
        }
    }

}
