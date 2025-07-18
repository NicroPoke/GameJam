using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ElectricBall : MonoBehaviour
{
    public GameObject lightningStrike; 
    private float startTime;
    private float timeToLive = 5f;
    private float strikeCooldown = 0.6f;
    private float strikeTimer = 0f;
    private bool canStrike = true;
    void Awake()
    {
        startTime = Time.time;
    }

    void Update()
    {
        strikeTimer += Time.deltaTime;

        if (Time.time - startTime >= timeToLive)
        {
            Destroy(gameObject);
        }

        if (strikeTimer >= strikeCooldown)
        {
            canStrike = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canStrike)
            {
                Vector2 playerPos = (Vector2)collision.transform.position;

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