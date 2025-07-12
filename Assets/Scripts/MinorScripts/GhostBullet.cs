using UnityEngine;

public class GhostBullet : MonoBehaviour
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
            Destroy(gameObject);
        }
    }
}
