using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class BossHandler : MonoBehaviour
{
    public GameObject[] ghosts;
    public GameObject[] bullets;
    public Transform target;

    private int health = 100;

    private float coolDown = 0.7f;
    private float timer = 0f;

    public float bulletSpeed = 20f;

    private int state = 1;
    private int numStates = 1;

    public float healthPoints = 100;

    public float bulletsLeft = 9;

    bool doesExplosionExists;
    GameObject pool;

    bool isLightningInstantiated = false;
    GameObject lightning;

    bool hasAtacked = false;

    

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        GetRandomState();
    }

    void Update()
    {

        switch (state)
        {
            case 1:
                ShotRound();
                break;
            case 2:
                break;
            case 3:
                GiantExplosion();
                break;
            case 4:
                Spew();
                break;
            case 5:
                ShotRound();
                break;
            case 6:
                SkeletonSpew();
                break;
            case 7:
                SpawnGhosts();
                break;
        }
    }

    void BulletBarrage()
    {
        if (bulletsLeft > 0)
        {
            timer += Time.deltaTime;
            if (timer > coolDown)
            {
                timer = 0;
                Shot(bullets[0]);
                bulletsLeft--;
            }
        }
        else
        {
            bulletsLeft = 9;
            GetRandomState();
        }
    }

    void SpawnGhosts()
    {
        if (!hasAtacked)
        {
            for (int i = 0; i < 4; i++)
            {
                Instantiate(ghosts[0], transform.position + 2 * transform.forward, transform.rotation);
            }
            hasAtacked = true;
        }
        else
        {
            if (timer > 6f)
            {
                timer = 0f;
                hasAtacked = false;
                GetRandomState();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    void GiantExplosion()
    {
        if (!doesExplosionExists)
        {
            pool = Instantiate(bullets[1], transform.position, transform.rotation);
            doesExplosionExists = true;
        }
        else
        {
            if (pool.transform.localScale.x >= 18f)
            {
                doesExplosionExists = false;
                Destroy(pool);

                GetRandomState();
            }
        }
    }

    void Spew()
    {
        if (!hasAtacked)
        {
            Vector2 offset = Vector2.zero;

            for (int i = 0; i < 10; i++)
            {
                offset = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
                Vector2 position = (Vector2)transform.position + offset;
                Instantiate(bullets[3], position, transform.rotation);
            }

            hasAtacked = true;
        }
        else
        {
            if (timer > 6f)
            {
                timer = 0f;
                hasAtacked = false;
                GetRandomState();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    List<Vector2> GenerateCircleVectors(int num)
    {
        List<Vector2> vectors = new List<Vector2>();
        float angleStep = 360f / num;

        for (int i = 0; i < num; i++)
        {
            float deg = i * angleStep * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(deg), Mathf.Sin(deg));
            vectors.Add(direction);
        }
        return vectors;
    }

    void ShotRound()
    {
        if (!hasAtacked)
        {
            List<Vector2> vectors = GenerateCircleVectors(6);

            foreach (Vector2 direction in vectors)
            {
                float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

                GameObject ghostBullet = Instantiate(bullets[4], transform.position, rotation);
                ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

            }

            hasAtacked = true;
        }
        else
        {
            if (timer > 3f)
            {
                timer = 0f;
                hasAtacked = false;
                GetRandomState();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
    }

    void SkeletonSpew()
    {
        if (!hasAtacked)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
                float angleOffset = Random.Range(-30f, 30f);
                Vector2 newDir = Quaternion.Euler(0, 0, angleOffset) * direction;

                float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);

                GameObject projectile = Instantiate(bullets[0], transform.position, rotation);
                projectile.GetComponent<Rigidbody2D>().linearVelocity = newDir * bulletSpeed * 4f;

                hasAtacked = true;
            }
        }
        else
        {
            if (timer > 2f)
            {
                timer = 0f;
                hasAtacked = false;
                GetRandomState();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

    }

    public void TakeDamege(int damage)
    {
        health -= damage;
    }

    // void ShotLightningSpark()
    // {
    //     if (!isLightningInstantiated)
    //     {
    //         Vector2 startPosition = (Vector2)transform.position;
    //         Vector2 direction = (startPosition - (Vector2)target.transform.position);

    //         Vector2 middlePoint = startPosition + direction / 2;

    //         float rotationZz = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //         Quaternion rotation = Quaternion.Euler(0, 0, rotationZz);

    //         lightning = Instantiate(bullets[2], middlePoint, rotation);

    //         lightning.GetComponent<RectTransform>().pivot = new Vector2(0, 0);

    //         isLightningInstantiated = true;
    //     }
    //     else if (isLightningInstantiated)
    //     {
    //         Vector3 scale = lightning.transform.localScale;
    //         float newX = Mathf.Lerp(scale.x, 20f, Time.deltaTime);
    //         lightning.transform.localScale = new Vector3(newX, scale.y, scale.z);


    //         if (lightning.transform.localScale.x >= 18)
    //         {
    //             isLightningInstantiated = false;
    //             Destroy(lightning);

    //             GetRandomState();
    //         }
    //     }

    // }

    void Shot(GameObject bullet)
    {
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);

        GameObject projectile = Instantiate(bullet, transform.position, rotation);
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed * 7f;
    }

    void GetRandomState()
    {
        state = Random.Range(1, numStates + 1);
        Debug.Log(state);
    }
}
