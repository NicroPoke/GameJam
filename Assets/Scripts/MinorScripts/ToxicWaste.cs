using UnityEngine;
using UnityEngine.Timeline;

 class ToxicWaste : MonoBehaviour
{
    public float halfLife = 5f;
    private float localTime;
    private Vector3 desiredSize = new Vector3(1.4f, 1.4f, 1.4f);
    void Awake()
    {
        localTime = Time.time;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime);

        if (transform.localScale == desiredSize) Destroy(gameObject);
    }
}
