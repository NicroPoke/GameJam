using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHandler : MonoBehaviour
{
    public GameObject[] ghosts;
    public GameObject[] bullets;
    public Transform target;

    [HideInInspector] public bool StartCD = true;
    private int health = 100;

    private float coolDown = 0.7f;
    private float timer = 0f;

    public float bulletSpeed = 20f;

    private int state = 1;
    private int numStates = 6;

    public float healthPoints = 175;

    public float bulletsLeft = 5;

    bool doesExplosionExists;
    GameObject pool;

    [HideInInspector] public bool isLightningInstantiated = false;
    GameObject lightning;

    bool hasAtacked = false;

    private int moveCount = 0;
    private bool hasSpawnedGhosts = false;

    [HideInInspector] public bool isDead = false;

    private Animator bossAnimator;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    public GameObject healthBar;
    public AudioSource player;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        bossAnimator = GetComponent<Animator>();
        GetRandomState();
    }

    void Update()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1.4f)
            {
                isAttacking = false;
                bossAnimator.SetBool("isAtak", false);
                attackTimer = 0f;
            }
        }

        if (!isLightningInstantiated) return;

        if (isDead) return;

        switch (state)
        {
            case 1:
                SpawnGhosts();
                break;
            case 2:
                BulletBarrage();
                break;
            case 3:
                GiantExplosion();
                break;
            case 4:
                Spew();
                break;
            case 5:
                BulletBarrage();
                break;
            case 6:
                SkeletonSpew();
                break;
            case 7:
                SpawnGhosts();
                break;
        }

        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        bossAnimator.SetBool("isDead", isDead);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            bossAnimator.SetBool("isAtak", true);
            player.Play();
        }
    }

    void BulletBarrage()
    {
        if (bulletsLeft > 0)
        {
            timer += Time.deltaTime;
            if (timer > 1.2f)
            {
                timer = 0;
                Shot(bullets[0]);
                bulletsLeft--;
            }
        }
        else
        {
            bulletsLeft = 5;
            GetRandomState();
        }
    }

    void SpawnGhosts()
    {
        if (!hasAtacked)
        {
            int numGhosts = Random.Range(1, 7);
            for (int i = 0; i < numGhosts; i++)
            {
                Instantiate(ghosts[Random.Range(0, 3)], transform.position + 2 * transform.forward, transform.rotation);
            }
            hasAtacked = true;
            hasSpawnedGhosts = true;
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
        if (!hasAtacked)
        {
            pool = Instantiate(bullets[1], transform.position, transform.rotation);
            hasAtacked = true;
        }
        else
        {
            if (pool.transform.localScale.x >= 2.4f)
            {
                hasAtacked = false;
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
            if (timer > 4f)
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
            isAttacking = true;
            attackTimer = 0f;
            List<Vector2> vectors = GenerateCircleVectors(6);

            foreach (Vector2 direction in vectors)
            {
                float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, rotationZ);

                GameObject ghostBullet = Instantiate(bullets[4], transform.position + (Vector3)direction * 0.5f, rotation);
                ghostBullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
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

    public void TakeDamege(int damage)
    {
        health -= damage;
        Debug.Log(health);
        healthBar.GetComponent<Image>().fillAmount = health / 100f; 
    }

    void Shot(GameObject bullet)
    {
        bossAnimator.SetBool("isAtak", true);
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);

        GameObject projectile = Instantiate(bullet, transform.position, rotation);
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed * 5.7f;
    }

    void GetRandomState()
    {
        moveCount++;

        if (moveCount % 3 == 0 && !hasSpawnedGhosts)
        {
            state = 7; 
        }
        else
        {
            state = Random.Range(1, numStates + 1);
            if (hasSpawnedGhosts && state == 7)
            {
                state = Random.Range(1, 7); 
            }
        }

        StartAttack();

        Debug.Log($"State selected: {state}");
    }
}
