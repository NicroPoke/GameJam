using UnityEngine;

public class Screamer : MonoBehaviour
{
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
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"OnTriggerStay2D at {Time.time:F2}s, isSlowed={isSlowed}, lastEffectTime={lastEffectTime:F2}");

        if (Time.time - lastEffectTime < invulnerabilityDuration)
        {
            Debug.Log("Invulnerability active, skipping attack.");
            isAttacking = false;
            return;
        }

        if (other.TryGetComponent(out PlayerController controller))
        {
            Debug.Log("Starting ApplySlowEffect");
            StartCoroutine(ApplySlowEffect(controller));
            isAttacking = true;
            lastEffectTime = Time.time;
        }
    }

    private System.Collections.IEnumerator ApplySlowEffect(PlayerController controller)
    {
        if (isSlowed)
        {
            Debug.Log("Already slowed, exiting coroutine.");
            yield break;
        }

        isSlowed = true;
        float originalSpeed = controller.speed;
        controller.speed = slowSpeed;
        Debug.Log($"Player slowed to {slowSpeed} at {Time.time:F2}s");

        yield return new WaitForSeconds(slowDuration);

        if (controller != null)
        {
            controller.speed = originalSpeed;
            Debug.Log($"Player speed restored to {originalSpeed} at {Time.time:F2}s");
        }

        isSlowed = false;
        isAttacking = false;
    }
}
