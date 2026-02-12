using UnityEditor;
using UnityEngine;

public class EditorUtils {
    /**
     * Finds the position of the cursor in world space.
     */
    public static Vector2 GetMouseWorldPosition()
    {
        Vector3 worldPosition = Event.current.mousePosition;

        Ray ray = HandleUtility.GUIPointToWorldRay(worldPosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        plane.Raycast(ray, out float dist);
        return ray.GetPoint(dist);
    }
}