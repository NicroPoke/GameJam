using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    private InventoryScroll inventoryScroll;
    private Camera cam;
    private float zoomSpeed = 5f;
    private float minZoom = 5f;
    private float maxZoom = 13f;
    private float smoothmovement = 3f;
    private float zoomLerpSpeed = 10f;
    private float targetZoom;

    private float shakeDuration = 0.3f;
    private float shakeMagnitude = 0.025f;
    private float shakeTimer = 0f;
    private float noiseSeed;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            inventoryScroll = player.GetComponent<InventoryScroll>();
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure there is a GameObject with the tag 'Player'.");
        }

        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        noiseSeed = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 posEnd = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        Vector3 posSmooth = Vector3.Lerp(transform.position, posEnd, smoothmovement * Time.deltaTime);

        if (inventoryScroll != null && inventoryScroll.isPulled && !inventoryScroll.isOverheat)
        {
            shakeTimer = shakeDuration;
        }

        if (shakeTimer > 0 && (inventoryScroll == null || !inventoryScroll.isOverheat))
        {
            float noiseX = Mathf.PerlinNoise(noiseSeed, Time.time * 20f) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(noiseSeed + 1f, Time.time * 20f) * 2f - 1f;
            Vector3 shakeOffset = new Vector3(noiseX, noiseY, 0) * shakeMagnitude;

            posSmooth += shakeOffset;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = posSmooth;

        if (Input.GetMouseButton(2))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                targetZoom -= scroll * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
        }

        if (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomLerpSpeed * Time.deltaTime);
        }
        else
        {
            cam.orthographicSize = targetZoom;
        }
    }
}
