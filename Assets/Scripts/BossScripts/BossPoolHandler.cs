using UnityEngine;

public class BossPoolHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float halfLife = 6f;
    private float localTime;
    private Vector3 desiredSize = new Vector3(2.5f, 2.5f, 2.5f);
    private bool canStrike;
    private float cdTimer = 0f;
    private float damageCD = 1f;
    void Awake()
    {
        localTime = Time.time;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime);

        if (cdTimer >= damageCD)
        {
            canStrike = true;
        }
        else
        {
            cdTimer += Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var controller = collision.gameObject.GetComponent<PlayerController>();

            controller.TakeDamege(10);
            canStrike = false;
            cdTimer = 0;
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (canStrike)
            {
                var controller = collision.gameObject.GetComponent<PlayerController>();
                controller.TakeDamege(10);

                canStrike = false;
                cdTimer = 0;
            }
        }
    }
}
