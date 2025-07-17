using System;
using System.Buffers.Text;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class GlitchBullet : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private GameObject debugSpherePrefab; 
    private float startTime;
    public float timeToLive = 5f;
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
        Vector2 origin = ghost.transform.position;

        for (int i = 0; i < 10; i++)
        {
            float random_x = UnityEngine.Random.Range(-4f, 4f);
            float random_y = UnityEngine.Random.Range(-2.5f, 2.5f);
            Vector2 newPos = origin + new Vector2(random_x, random_y);

            Vector2 direction = (newPos - origin).normalized;
            float dist = Vector2.Distance(origin, newPos);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, dist);

            if (hit.collider == null || hit.collider.gameObject.tag == "Ghost")
            {
                Debug.Log("Found position: " + newPos);
                ghost.GetComponent<BaseGhost>().teleport(newPos);
                return;
            }
        }

        Debug.LogWarning("Не удалось телепортироваться: нет свободной точки");
    }

    void SpawnDebugSphere(Vector2 position)
    {
        if (debugSpherePrefab != null)
        {
            var sphere = Instantiate(debugSpherePrefab, position, Quaternion.identity);
            Destroy(sphere, 2f);
        }
        else
        {
            Debug.LogWarning("debugSpherePrefab не назначен!");
        }
    }
}
