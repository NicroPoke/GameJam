using UnityEngine;

public class ContactGhost : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Movement Settings")]
    [HideInInspector] public float Speed = 5f;
    [HideInInspector] public float Amplitude = 0.1f;
    [HideInInspector] public float Frequency = 2f;

    [Header("Damage Settings")]
    [HideInInspector] public float invulnerabilityDuration = 1.5f;

    private Vector3 velocityPosition;
    private float floatTimer;
    private float lastDamageTime = -Mathf.Infinity;

    void Start()
    {
        velocityPosition = transform.position;
    }

    void Update()
    {
        if (target == null)
            return;

        Vector3 direction = (target.position - velocityPosition).normalized;
        velocityPosition += direction * Speed * Time.deltaTime;
        floatTimer += Time.deltaTime;
        float floatOffset = Mathf.Cos(floatTimer * Frequency) * Amplitude;
        transform.position = velocityPosition + new Vector3(0f, floatOffset, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time - lastDamageTime < invulnerabilityDuration)
            return;

        if (collision.gameObject.TryGetComponent(out PlayerController controller))
        {
            controller.TakeDamege(1);  
            lastDamageTime = Time.time;
        }
    }
}