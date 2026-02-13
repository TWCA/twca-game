using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelPortal))]
public class LevelPortalEditor : Editor {
    private float nodeRadius = 7f;
    private bool dragging = false;
    public void OnSceneGUI() {
        LevelPortal root = target as LevelPortal;

        Handles.color = Color.white;
        if (Handles.Button(root.PortalPosition, Quaternion.identity, nodeRadius, nodeRadius, Handles.CircleHandleCap)) {
            dragging = !dragging;
        }

        Handles.DrawSolidDisc(root.PortalPosition, Vector3.forward, nodeRadius);

        if (dragging) {
            root.PortalPosition = EditorUtils.GetMouseWorldPosition();
            EditorUtility.SetDirty(root);
        }
    }
}