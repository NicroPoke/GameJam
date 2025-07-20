using UnityEngine;

public class ChildBullet : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    private float timeToBeTriggered = 3f;
    public GameObject corpse;

    public float rotationSpeed = 1000f;

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        startTime = Time.time;
    }

    void Update()
    {
        // вращение по оси Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

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
        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossHandler>().TakeDamege(1);
            Destroy(gameObject);
        }
        if (collision.gameObject == corpse) return;

        if (collision.gameObject.CompareTag("Ghost") || collision.gameObject.CompareTag("Angel"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();

            Rigidbody2D _rb = GetComponent<Rigidbody2D>();
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 10;

            controller.ApplyExternalForce(transform.right * 30);
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