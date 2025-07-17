using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampSwinging2D : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float angle = 7f;
    [SerializeField] private float minorAngle = 0.5f;
    private float baseZRotation;
    private float lampOffset;
    private float phaseOffset;
    private float minorPhaseOffset;
    [SerializeField] private bool lampLag = true;

    private Light2D myLight;

    private float flickerTime;
    private bool isFlickering;
    private float flickerDuration = 0.35f;
    private float steadyDuration;

    void Start()
    {
        lampLag = true;
        baseZRotation = transform.eulerAngles.z;
        lampOffset = Random.Range(-angle * 0.5f, angle * 0.5f);
        phaseOffset = Random.Range(0f, Mathf.PI * 2);
        minorPhaseOffset = Random.Range(0f, Mathf.PI / 2);
        myLight = GetComponent<Light2D>();
        isFlickering = true;
        steadyDuration = Random.Range(2f, 7f);
    }

    void Update()
    {
        Debug.Log(lampLag);
        float t = (Mathf.Sin(Time.time * speed + phaseOffset) + 1) * 0.5f;
        float mainSwing = Mathf.Lerp(-angle, angle, t) + lampOffset;

        float minorT = (Mathf.Sin(Time.time * (speed * 0.75f) + minorPhaseOffset) + 1) * 0.5f;
        float minorSwing = Mathf.Lerp(-minorAngle, minorAngle, minorT);

        transform.rotation = Quaternion.Euler(0f, 0f, baseZRotation + mainSwing + minorSwing);

        if (lampLag && myLight != null)
        {
            flickerTime += Time.deltaTime;

            if (isFlickering)
            {
                myLight.intensity = Mathf.Lerp(1.5f, 2.5f, Mathf.PingPong(Time.time * 10f, 1));
                if (flickerTime > flickerDuration)
                {
                    flickerTime = 0f;
                    isFlickering = false;
                }
            }
            else
            {
                myLight.intensity = 2.5f;
                if (flickerTime > steadyDuration)
                {
                    flickerTime = 0f;
                    isFlickering = true;
                    steadyDuration = Random.Range(3f, 7f);
                }
            }
        }
    }
}