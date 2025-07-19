using UnityEngine;

public class ToxicWater : MonoBehaviour
{
    public float halfLife = 12f;
    private float localTime;
    private Vector3 desiredSize = new Vector3(2.5f, 2.5f, 2.5f);
    void Awake()
    {
        localTime = Time.time;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime * 1.5f);

        if (Time.time - localTime <= halfLife) Destroy(gameObject);
    }
}
