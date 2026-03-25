using UnityEngine;

public static class RaycastManager
{
    public static int IgnoreLayer = 2;
    private static int IgnoreLayerMask = 1 << IgnoreLayer;
    private static RaycastHit2D CalculateHit() {
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 ray = cameraPos;
        RaycastHit2D hit = Physics2D.Raycast(ray, ray, ~IgnoreLayerMask);

        return hit;
    }
    public static T IsComponentBelowMouse<T>(T def = default) {
        RaycastHit2D hit = CalculateHit();

        if (hit.collider != null)
        {
            T component = hit.collider.gameObject.GetComponent<T>();

            if (component != null) {
                return component;
            }
        }

        return def;
    }

    public static bool IsGameObjectBelowMouse(GameObject gameObject) {
        RaycastHit2D hit = CalculateHit();

        if (hit.collider != null)
        {
            return hit.collider.gameObject == gameObject;
        }

        return false;
    }
}
