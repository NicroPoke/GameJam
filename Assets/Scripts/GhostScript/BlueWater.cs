using UnityEngine;

public class Toxines : MonoBehaviour
{
    private float lastDamageTime = -Mathf.Infinity;
    private float invulnerabilityDuration = 0.5f;
    private bool isAttacking = false;

    void Awake()
    {
        GetComponent<CircleCollider2D>().includeLayers = LayerMask.GetMask("Default");
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
            controller.TakeDamegePool(1);
            controller.speed = 2f;
            isAttacking = true;
            Debug.Log("Contact with player.");
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
    private void Update()
    {
        Destroy(gameObject, 10f);
    }
}

