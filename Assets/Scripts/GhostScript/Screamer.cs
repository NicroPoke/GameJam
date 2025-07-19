using UnityEngine;

public class Screamer : MonoBehaviour
{
    private float lastDamageTime = -Mathf.Infinity;
    private float lastEffectTime = -Mathf.Infinity;
    private float invulnerabilityDuration = 2f;
    private bool isAttacking = false;
    private bool isSlowed = false;

    [SerializeField] private float slowDuration = 2f;
    [SerializeField] private float slowSpeed = 1f;
    private Transform player;

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 dir = player.position - transform.position;
            float angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
        {
            isAttacking = false;
            return;
        }

        if (other.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(5); 
            controller.speed = 2f;
            isAttacking = true;
            Debug.Log("Screamer: contact with player and damage dealt.");
            lastDamageTime = Time.time;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (other.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.speed = 6f;
        }
    }
}
