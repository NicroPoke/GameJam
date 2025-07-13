using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class FurryBullet : MonoBehaviour
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

            Debug.Log(_rb.linearVelocity.normalized);

            controller.ApplyExternalForce(transform.right * 30);

            FindNewDirection(collision.gameObject);
        }
    }

    void FindNewDirection(GameObject collider)
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        GameObject closest = FindClosest(ghosts, collider);

        Debug.Log(closest.name);

        if (closest != null)
        {
            Vector2 direction = ((Vector2)closest.transform.position - (Vector2)transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = direction * 3;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    GameObject FindClosest(GameObject[] ghosts, GameObject current)
    {
        if (ghosts.Length <= 1)
        {
            return null;
        }
        GameObject currentObject = null;
        foreach (GameObject ghost in ghosts)
        {
            if (ghost == current)
                break;

            if (currentObject == null)
                currentObject = ghost;

            if (isCloser(currentObject, ghost))
                currentObject = ghost;
        }

        return currentObject;
    }

    bool isCloser(GameObject current, GameObject contestor)
    {
        float currentDistance = Vector2.Distance((Vector2)transform.position, (Vector2)current.gameObject.transform.position);
        float contestedDistane = Vector2.Distance((Vector2)transform.position, (Vector2)contestor.gameObject.transform.position);

        if (contestedDistane < currentDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
