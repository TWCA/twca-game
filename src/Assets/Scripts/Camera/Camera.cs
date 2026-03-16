using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 8f;
    public Vector2 minBounds;
    public Vector2 maxBounds;
    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
        float clampedX = Mathf.Clamp(desired.x, minBounds.x + camHalfWidth,  maxBounds.x - camHalfWidth);
        float clampedY = Mathf.Clamp(desired.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        Vector3 finalPos = new Vector3(clampedX, clampedY, desired.z);
        transform.position = Vector3.Lerp(transform.position, finalPos, smoothSpeed * Time.deltaTime);
    }
}