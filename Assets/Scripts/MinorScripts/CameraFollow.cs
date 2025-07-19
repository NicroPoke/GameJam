using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    private InventoryScroll inventoryScroll;
    private Camera cam;
    private float zoomSpeed = 5f;
    private float minZoom = 5f;
    private float maxZoom = 10f;
    private float smoothmovement = 3f;
    private float zoomLerpSpeed = 10f;
    private float targetZoom;

    private float shakeDuration = 0.3f;
    private float shakeMagnitude = 0.025f;
    private float shakeTimer = 0f;
    private float noiseSeed;

    private bool isLockedToPoint = false;
    private Transform lockTarget;
    private float lockTransitionSpeed = 2f;

    private float lockedZoom = 12f;

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            inventoryScroll = player.GetComponent<InventoryScroll>();
        }

        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        noiseSeed = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = transform.position;

        if (isLockedToPoint && lockTarget != null)
        {
            Vector3 midPoint = lockTarget.position;

            if (player != null)
            {
                Vector3 toPlayer = player.transform.position - lockTarget.position;
                midPoint += toPlayer * 0.2f;
            }

            Vector3 targetPos = new Vector3(midPoint.x, midPoint.y, transform.position.z);
            targetPosition = Vector3.Lerp(transform.position, targetPos, lockTransitionSpeed * Time.deltaTime);

            if (Mathf.Abs(targetZoom - lockedZoom) > 0.01f)
            {
                targetZoom = Mathf.Lerp(targetZoom, lockedZoom, zoomLerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (player == null) return;

            Vector3 posEnd = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
            targetPosition = Vector3.Lerp(transform.position, posEnd, smoothmovement * Time.deltaTime);
        }

        if (inventoryScroll != null && inventoryScroll.isPulled && !inventoryScroll.isOverheat)
        {
            shakeTimer = shakeDuration;
        }

        if (shakeTimer > 0 && (inventoryScroll == null || !inventoryScroll.isOverheat))
        {
            float noiseX = Mathf.PerlinNoise(noiseSeed, Time.time * 20f) * 2f - 1f;
            float noiseY = Mathf.PerlinNoise(noiseSeed + 1f, Time.time * 20f) * 2f - 1f;
            Vector3 shakeOffset = new Vector3(noiseX, noiseY, 0) * shakeMagnitude;

            targetPosition += shakeOffset;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = targetPosition;

        if (Input.GetMouseButton(2))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            if (mouseY != 0f)
            {
                targetZoom -= mouseY * zoomSpeed;
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

    public void ShakeExternal(float duration = 0.15f, float magnitude = 0.1f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimer = duration;
    }

    public void LockToTransform(Transform target)
    {
        lockTarget = target;
        isLockedToPoint = true;
    }
}
