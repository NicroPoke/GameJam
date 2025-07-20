using UnityEngine;

public class ElectricSPinner : MonoBehaviour
{
private float startTime;

    public GameObject corpse;

    public float rotationSpeed = 1000f;

    void Awake()
    {
        startTime = Time.time;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
