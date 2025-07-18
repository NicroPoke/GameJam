using UnityEngine;

public class CameraLockTrigger : MonoBehaviour
{
    public Transform targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null && targetObject != null)
            {
                camFollow.LockToTransform(targetObject);
            }
        }
    }
}