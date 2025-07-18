using UnityEngine;

public class AngelBullet : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    void Awake()
    {
        startTime = Time.time;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<PlayerController>().TakeDamege(-15);
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

            Destroy(gameObject);
        }
    }
}
