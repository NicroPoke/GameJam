using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class HomelessBullet : MonoBehaviour
{
    public GameObject electricField;
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
        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossHandler>().TakeDamege(7);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ghost") || collision.gameObject.CompareTag("Angel"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();
            controller.ApplyExternalForce(transform.right * 30);

            

            GameObject objectoe = Instantiate(electricField, transform.position, transform.rotation);
            Debug.Log(objectoe);

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
