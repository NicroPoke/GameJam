using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class ConsumableScript : MonoBehaviour
{
    private float respawnTimer = 0;
    private float respawnCooldown = 5f;
    private bool isRespawning = false;
    public GameObject respawnable;
    public string consumableName;

    void Awake()
    {
        if (name == "") return;

        gameObject.name = consumableName;

        if (respawnable != null)
        {
            isRespawning = true;
        }
    }

    void Update()
    {
        if (isRespawning)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnCooldown)
            {
                Respawn();
                
                Destroy(gameObject);
            }
        }
    }

    void Respawn()
    {
        GameObject ghost = Instantiate(respawnable, transform.position, transform.rotation);
        ghost.GetComponent<BaseGhost>().Alive = true;
        ghost.GetComponent<BaseGhost>().target = GameObject.FindGameObjectWithTag("Player").transform;
        ghost.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        ghost.GetComponent<SpriteRenderer>().flipX = true;
        ghost.GetComponent<SkeletonGhost>().isRespawning = true;
    }
}