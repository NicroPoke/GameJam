using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkeletonBullet : MonoBehaviour
{
    [SerializeField] private GameObject basicBullet;
    private float startTime;
    private float timeToLive = 3f;
    private float timeToBeTriggered = 3f;
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
        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<BossHandler>().TakeDamege(7);
            SpawnAditionaBullets(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ghost") || collision.gameObject.CompareTag("Angel"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();
            controller.ApplyExternalForce(transform.right * 30);

            SpawnAditionaBullets(collision.gameObject);

            Destroy(gameObject);
        }
    }

    void SpawnAditionaBullets(GameObject corpse)
    {
        int bulletNumber = (int)Random.Range(2, 4);

        for (int i = 0; i <= bulletNumber; i++)
        {
            Debug.Log("Spawned");
            float direction_y = Random.Range(-1f, 1f);
            float direction_x = Random.Range(-1f, 1f);

            Vector2 direction = new Vector2(direction_x, direction_y).normalized;


            float rotationZ = Mathf.Atan2(direction_y, direction_x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

            GameObject ghostBullet = Instantiate(basicBullet, transform.position, rotation);

            ghostBullet.GetComponent<ChildBullet>().corpse = corpse;
            float baseScale = Random.Range(0.4f, 0.8f);
            ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = direction * Random.Range(1.5f, 4f);
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
