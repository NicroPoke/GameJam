using UnityEngine;

public class WalllCollisionController : MonoBehaviour
{
    public Collider2D triggerCollider;
    public Collider2D regularCollider;

    void Awake()
    {
        triggerCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Created");
            triggerCollider.enabled = false;
            regularCollider.enabled = true;
        }
        else
        {
            triggerCollider.enabled = true;
            regularCollider.enabled = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        triggerCollider.enabled = true;
        regularCollider.enabled = false;
    }
}
