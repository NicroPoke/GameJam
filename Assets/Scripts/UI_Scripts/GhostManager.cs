using UnityEngine;
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

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

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
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < GhostsPerWave; i++)
        {
            int randomIndex = Random.Range(0, Ghosts.Count);
            GameObject selectedGhost = Ghosts[randomIndex];

            Vector3 spawnPos;

            for (int attempts = 0; attempts < 10; attempts++)
            {
                Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(5f, spawnRadius);
                spawnPos = playerTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

                Collider2D groundCheck = Physics2D.OverlapCircle(spawnPos, 0.1f, groundLayer);
                if (groundCheck != null)
                {
                    Instantiate(selectedGhost, spawnPos, Quaternion.identity);
                    break;
                }
            }

            yield return new WaitForSeconds(0.5f);
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
