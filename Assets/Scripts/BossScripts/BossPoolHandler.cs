using UnityEngine;

public class BossPoolHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float halfLife = 6f;
    private float localTime;
    private Vector3 desiredSize = new Vector3(20f, 20f, 20f);
    void Awake()
    {
        localTime = Time.time;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var controller = collision.gameObject.GetComponent<PlayerController>();

            controller.TakeDamege(30);
        }
    }
}
