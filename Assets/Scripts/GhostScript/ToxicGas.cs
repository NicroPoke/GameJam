using UnityEngine;

public class ToxicGas : MonoBehaviour
{
    private float lastDamageTime = -Mathf.Infinity;
    private float invulnerabilityDuration = 0.5f;
    private bool isAttacking = false;

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
            controller.TakeDamege(1);
            isAttacking = true;
            Debug.Log("Contact with player.");
            lastDamageTime = Time.time;
        }
    }
}

