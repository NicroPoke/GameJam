using System;
using System.Collections;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class LoudBullet : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    void Awake()
    {
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
        if (collision.gameObject.CompareTag("Ghost"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();

            Rigidbody2D _rb = GetComponent<Rigidbody2D>();

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 10;

            controller.ApplyExternalForce(transform.right * 30);

            controller.ApplySlow(3, 3);

            Destroy(gameObject);
        }
    }

    IEnumerator SlowDown(BaseGhost controller)
    {
        controller.Speed -= 3;

        yield return new WaitForSeconds(3f);

        controller.Speed += 3;
    }

}
