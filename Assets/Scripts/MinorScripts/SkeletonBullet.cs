using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkeletonBullet : MonoBehaviour
{
    [SerializeField] private GameObject basicBullet;
    private float startTime;
    private float timeToLive = 5f;
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
        if (collision.gameObject.CompareTag("Ghost"))
        {
            var controller = collision.gameObject.GetComponent<BaseGhost>();
            controller.ApplyExternalForce(transform.right * 30);

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            SpawnAditionaBullets();

            Destroy(gameObject);
        }
    }

    void SpawnAditionaBullets()
    {
        int bulletNumber = (int)Random.Range(3, 6);

        for (int i = 0; i <= bulletNumber; i++)
        {
            float direction_y = Random.Range(-1f, 1f);
            float direction_x = Random.Range(-1f, 1f);

            Debug.Log(direction_y);
            Debug.Log(direction_x);

            Vector2 direction = new Vector2(direction_x, direction_y).normalized;


            float rotationZ = Mathf.Atan2(direction_y, direction_x) * Mathf.Rad2Deg;

            Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

            GameObject ghostBullet = Instantiate(basicBullet, transform.position, rotation);

            float baseScale = Random.Range(0.4f, 0.8f);
            ghostBullet.transform.localScale = new Vector3(baseScale * 0.5f, baseScale, 1f);
            ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = direction * Random.Range(1.5f, 4f);
        }
    }

}
