using System;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchBullet : MonoBehaviour
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
            teleportToRandomLocation(collision.gameObject);

            Destroy(gameObject);
        }
    }

    void teleportToRandomLocation(GameObject ghost)
    {
        Vector3 currentPosition = transform.position;

        int i = 0;

        while (i < 10)
        {
            float random_x = UnityEngine.Random.Range(-4f, 4f);
            float random_y = UnityEngine.Random.Range(-2.5f, 2.5f);
            Vector3 newPosition = currentPosition + new Vector3(random_x, random_y, 0);

            Vector2 direction = (newPosition - currentPosition).normalized;
            float distance = Vector2.Distance(newPosition, currentPosition);

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, distance);
            if (hit.collider == null)
            {
                Debug.Log("Found");
                ghost.transform.position = newPosition;
                return;
            }

            i++;
        }

    }
}
