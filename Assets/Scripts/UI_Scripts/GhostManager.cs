using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance;
    public GameObject BaseGhost;
    public List<GameObject> Ghosts;
    public int Waves = 3;
    public int GhostsPerWave = 5;
    public float spawnRadius = 5f;
    public LayerMask groundLayer;

    private int currentWave = 0;
    private bool coroutineRunning = false;
    private bool waveSpawning = false;
    private Transform playerTransform;
    private bool initialGhostsCleared = false;
    private bool allWavesCompleted = false;

    public bool battleEnded { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        string targetTag = SceneManager.GetActiveScene().buildIndex == 7 ? "Boss" : "Ghost";
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag(targetTag);

        if (!initialGhostsCleared)
        {
            if (ghosts.Length == 0)
            {
                initialGhostsCleared = true;
                StartCoroutine(SpawnWave());
            }
        }
        else if (!waveSpawning && ghosts.Length == 0 && currentWave < Waves)
        {
            StartCoroutine(SpawnWave());
        }
        else if (ghosts.Length == 0 && currentWave >= Waves && allWavesCompleted)
        {
            battleEnded = true;
        }

        if (!coroutineRunning)
        {
            StartCoroutine(SpawnBaseGhost());
        }
    }

    IEnumerator SpawnBaseGhost()
    {
        coroutineRunning = true;
        if (BaseGhost == null || playerTransform == null) yield break;

        yield return new WaitForSeconds(15f);

        Vector3 spawnPos;

        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            spawnPos = playerTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            Collider2D groundCheck = Physics2D.OverlapCircle(spawnPos, 0.1f, groundLayer);
            if (groundCheck != null)
            {
                Instantiate(BaseGhost, spawnPos, Quaternion.identity);
                break;
            }
        }

        coroutineRunning = false;
    }

    IEnumerator SpawnWave()
    {
        waveSpawning = true;

        float safeTime = 20f;
        float waveDelay = Time.timeSinceLevelLoad >= safeTime ? 0f : 2f;
        yield return new WaitForSeconds(waveDelay);

        int ghostsToSpawn = GhostsPerWave;
        float dynamicRadius = Mathf.Lerp(spawnRadius, 2f, (float)currentWave / Waves);

        for (int i = 0; i < ghostsToSpawn; i++)
        {
            int randomIndex = Random.Range(0, Ghosts.Count);
            GameObject selectedGhost = Ghosts[randomIndex];

            float angle = i * (360f / ghostsToSpawn);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector3 spawnPos = playerTransform.position + (Vector3)(direction * dynamicRadius);

            Vector2 toSpawnDir = (spawnPos - playerTransform.position).normalized;
            float angleToPlayerForward = Vector2.Angle(playerTransform.right, toSpawnDir);

            if (angleToPlayerForward < 60f)
            {
                direction = Quaternion.Euler(0, 0, 90f) * direction;
                spawnPos = playerTransform.position + (Vector3)(direction * dynamicRadius);
            }

            for (int attempts = 0; attempts < 5; attempts++)
            {
                Collider2D groundCheck = Physics2D.OverlapCircle(spawnPos, 0.1f, groundLayer);
                if (groundCheck != null)
                {
                    Instantiate(selectedGhost, spawnPos, Quaternion.identity);
                    break;
                }
                spawnPos += (Vector3)(Random.insideUnitCircle * 1f);
            }

            yield return new WaitForSeconds(0.4f);
        }

        currentWave++;
        if (currentWave >= Waves)
        {
            allWavesCompleted = true;
        }
        waveSpawning = false;
    }

    public void ResetBattleFlag()
    {
        battleEnded = false;
        coroutineRunning = false;
        waveSpawning = false;
        initialGhostsCleared = false;
        allWavesCompleted = false;
        currentWave = 0;
    }
}
