using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    private InventoryScroll inventoryScroll;
    private Camera cam;
    private float zoomSpeed = 5f;
    private float minZoom = 5f;
    private float maxZoom = 13f;
    private float smoothmovement = 3f;
    private float zoomLerpSpeed = 10f;
    private float targetZoom;
    
    private float shakeDuration = 0.3f;
    private float shakeMagnitude = 0.05f;
    private float shakeTimer = 0f;
    private float noiseSeed;

    void Start()
    {
        inventoryScroll = player.GetComponent<InventoryScroll>();
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        noiseSeed = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        Vector3 posEnd = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        Vector3 posSmooth = Vector3.Lerp(transform.position, posEnd, smoothmovement * Time.deltaTime);

        if (inventoryScroll != null && inventoryScroll.isPulled)
        {
            shakeTimer = shakeDuration;
        }

        if (shakeTimer > 0)
        {
            float noiseX = Mathf.PerlinNoise(noiseSeed, Time.time * 20f) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(noiseSeed + 1f, Time.time * 20f) * 2f - 1f;
            Vector3 shakeOffset = new Vector3(noiseX, noiseY, 0) * shakeMagnitude;

            posSmooth += shakeOffset;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = posSmooth;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomLerpSpeed * Time.deltaTime);
    }
}
