using UnityEngine;

public class DummyEnepyScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided");
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerController>().TakeDamege(30);
            Debug.Log("Collided");
        }
    }
}
