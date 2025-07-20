using Unity.VisualScripting;
using UnityEngine;

public class BossLightningStrike : MonoBehaviour
{
    public GameObject lightningStrike; 
    private float startTime;
    private float timeToLive = 4f;
    private float strikeCooldown = 0.6f;
    private float strikeTimer = 0f;
    private Vector3 desiredSize = new Vector3(4.3f, 4.3f, 4.3f);

    public float desiredColSize = 13f;
    public float initialRadius = 0.3f;
    private CircleCollider2D cirCollider;

    private bool canStrike = true;
    void Awake()
    {
        startTime = Time.time;

    }

    void Update()
    {
        float growthDuration = 6f;
        float t = 0;
        t = Mathf.Clamp01((Time.time - startTime) / growthDuration);
        t = Mathf.SmoothStep(0f, 1f, t);
        transform.localScale = Vector3.Lerp(new Vector3(0.0001f, 0.0001f, 0.0001f), desiredSize, t);
        if (Time.time - startTime >= timeToLive)
        {
            Destroy(gameObject);
        }

        if (strikeTimer >= strikeCooldown)
        {
            canStrike = true;
        }
        else
        {
            strikeTimer += Time.deltaTime;
        }

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canStrike)
            {
                Vector2 playerPos = (Vector2)collision.transform.position;
                collision.gameObject.GetComponent<PlayerController>().TakeDamege(5);

                ShotLightningSpark(playerPos);
                canStrike = false;
                strikeTimer = 0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canStrike)
            {
                Vector2 playerPos = (Vector2)collision.transform.position;
                collision.gameObject.GetComponent<PlayerController>().TakeDamege(5);
                ShotLightningSpark(playerPos);
                canStrike = false;  
                strikeTimer = 0f;
            }

        }
    }

    void ShotLightningSpark(Vector2 playerPosition)
    {
        Vector2 startPosition = (Vector2)transform.position;
        Vector2 direction = (playerPosition - startPosition);

        Vector2 middlePoint = startPosition + direction / 2;

        float rotationZz = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, rotationZz);

        GameObject lightning = Instantiate(lightningStrike, middlePoint, rotation);

        float length = direction.magnitude;
        lightning.transform.localScale = new Vector3(length, lightning.transform.localScale.y, 1);
    }
}
