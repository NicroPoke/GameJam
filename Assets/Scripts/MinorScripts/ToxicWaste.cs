using UnityEngine;
using UnityEngine.Timeline;

 class ToxicWaste : MonoBehaviour
{
    public float halfLife = 2f;
    private float localTime;
    private Vector3 desiredSize = new Vector3(1.2f, 1.2f, 1.2f);
    void Awake()
    {
        localTime = Time.time;
    }

    void Update()
    {
        if (Time.time - localTime >= halfLife) Destroy(gameObject);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredSize, Time.deltaTime);

    }
}
