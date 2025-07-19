using System;
using UnityEngine;
using System.Collections.Generic;

public class FurryBullet : MonoBehaviour
{
    private int targetsLeft = 3;
    private float startTime;
    private float timeToLive = 5f;
    private GameObject[] ghosts;
    private Rigidbody2D rb;
    private float bulletSpeed = 5f;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();
    [HideInInspector] public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        startTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
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
        if (!collision.gameObject.CompareTag("Ghost") && !collision.gameObject.CompareTag("Angel"))
            return;

        if (alreadyHit.Contains(collision.gameObject))
            return;

        alreadyHit.Add(collision.gameObject);

        var controller = collision.gameObject.GetComponent<BaseGhost>();
        if (controller != null)
        {
            controller.ApplyExternalForce(transform.right * 30);
        }

        Rigidbody2D hitRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (hitRb != null)
        {
            hitRb.gravityScale = 10;
        }

        targetsLeft--;
        FindNewDirection(collision.gameObject);
    }

    void FindNewDirection(GameObject hit)
    {
        if (targetsLeft <= 0)
        {
            Destroy(gameObject);
            return;
        }

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        GameObject closest = FindClosest(ghosts, hit);

        if (closest != null)
        {
            Vector2 direction = ((Vector2)closest.transform.position - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rb.linearVelocity = direction * bulletSpeed * 3f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    GameObject FindClosest(GameObject[] objects, GameObject ignore)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector2 currentPosition = transform.position;

        foreach (GameObject obj in objects)
        {
            if (obj == ignore) continue;

            float dist = Vector2.Distance(currentPosition, obj.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = obj;
            }
        }

        return closest;
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
