using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    private Camera cam;
    private float zoomSpeed = 5f;
    private float minZoom = 5f;
    private float maxZoom = 13f;
    private float smoothmovement = 3f;
    private float zoomLerpSpeed = 10f;
    private float targetZoom;

    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
    }

    void FixedUpdate()
    {
        Vector3 posEnd = new Vector3(player.transform.position.x, player.transform.position.y, -10f); 
        Vector3 posSmooth = Vector3.Lerp(transform.position, posEnd, smoothmovement * Time.deltaTime);
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
