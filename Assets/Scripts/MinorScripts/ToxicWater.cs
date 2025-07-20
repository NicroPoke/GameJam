using UnityEngine;

public class ToxicWater : MonoBehaviour
{
    public float halfLife = 12f;
    private float localTime;
    private Vector3 desiredSize = new Vector3(0.6f, 0.6f, 0.6f);
    void Awake()
    {
        localTime = Time.time;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime * 1.5f);

        if (Time.time - localTime >= halfLife) Destroy(gameObject);
    }
}
