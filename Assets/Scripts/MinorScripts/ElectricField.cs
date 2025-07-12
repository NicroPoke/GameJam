using UnityEngine;

public class ElectricField : MonoBehaviour
{
    private float startTime;
    private float timeToLive = 5f;
    private Vector3 desiredSize = new Vector3(3f, 3f, 3f);

    void Awake()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime >= timeToLive)
        {
            Destroy(gameObject);
        }

        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime);
    }
}
