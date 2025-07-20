using UnityEngine;

public class ElectricField : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    private Vector3 desiredSize = new Vector3(1.4f, 1.4f, 1.4f);

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

        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime);
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
