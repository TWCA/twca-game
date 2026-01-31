using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathNetwork))]
public class PathNetworkEditor : Editor
{
    private const float NodeRadius = 7f;
    private const float ButtonSize = 5f;
    private const float ButtonSpacing = 12f;
    private const float EditRange = 60f;

    private int draggingIndex = -1;

    /**
     * Draws handles in the editor GUI to manipulate the path network.
     */
    public void OnSceneGUI()
    {
        PathNetwork net = target as PathNetwork;
        
        if (net.GetNodeCount() == 0)
        {
            net.CreateNode(Vector2.zero);
            EditorUtility.SetDirty(net);
        }

        Vector2 mousePosition = GetMouseWorldPosition();
        DrawPaths(mousePosition);
        DrawNodes(mousePosition);
    }

    /**
     * Draws the paths, and all their handles.
     */
    private void DrawPaths(Vector2 mousePosition)
    {
        PathNetwork net = target as PathNetwork;

        for (int i = 0; i < net.GetPathCount(); i++)
        {
            Vector2 positionA = net.GetPathPositionA(i);
            Vector2 positionB = net.GetPathPositionB(i);
            Vector2 midpoint = (positionA + positionB) / 2;

            if (net.IsPathTraversableAtTime(i, true))
            {
                if (net.IsPathTraversableAtTime(i, false))
                {
                    // always traversable
                    Handles.color = Color.white;
                    Handles.DrawLine(positionA, positionB);
                }
                else
                {
                    // traversable in the future
                    Handles.color = Color.cyan;
                    Handles.DrawDottedLine(positionA, positionB, 3.0f);
                }
            }
            else
            {
                if (net.IsPathTraversableAtTime(i, false))
                {
                    // traversable in the past
                    Handles.color = Color.yellow;
                    Handles.DrawDottedLine(positionA, positionB, 3.0f);
                }
                else
                {
                    // never traversable
                    Handles.color = Color.black;
                    Handles.DrawDottedLine(positionA, positionB, 3.0f);
                }
            }

            DrawText(midpoint + new Vector2(0, ButtonSpacing / 2), net.GetPathName(i));

            // hide UI far away from the mouse
            if (Vector2.Distance(mousePosition, midpoint) > EditRange) continue;
            // hide the buttons while dragging
            if (IsDragging()) continue;

            Handles.color = Color.green;
            if (DrawButton(midpoint + new Vector2(-ButtonSpacing, -ButtonSpacing / 2), "+", false))
            {
                BeginDrag(net.BreakPath(i));
                EditorUtility.SetDirty(net);
            }

            Handles.color = Color.magenta;
            if (DrawButton(midpoint + new Vector2(0, -ButtonSpacing / 2), "✎", false))
            {
                FocusPath(i);
            }

            Handles.color = Color.red;
            if (DrawButton(midpoint + new Vector2(ButtonSpacing, -ButtonSpacing / 2), "-", false))
            {
                net.DeletePath(i);
                EditorUtility.SetDirty(net);
                i--;
            }
        }
    }

    /**
     * Expands a path in the inspector to edit properties in detail. Closes all other paths.
     */
    private void FocusPath(int path)
    {
        SerializedProperty pathsProperty = serializedObject.FindProperty("paths");
        pathsProperty.isExpanded = true;

        // close all others paths, except the selected one
        for (int i = 0; i < pathsProperty.arraySize; i++)
        {
            SerializedProperty pathProperty = pathsProperty.GetArrayElementAtIndex(i);
            // make sure the focused path is expanded, and all others are not
            pathProperty.isExpanded = i == path;
        }

        // no idea why we have to call this twice
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        EditorApplication.delayCall += UnityEditorInternal.InternalEditorUtility.RepaintAllViews;
    }

    private void DrawNodes(Vector2 mousePosition)
    {
        PathNetwork net = target as PathNetwork;

        // if a node is being dragged it must be drawn first
        // this allows other nodes to be drawn over then, and thus the other node can be clicked to merge
        if (IsDragging())
            DrawDragHandle(draggingIndex, mousePosition);

        Handles.color = Color.white;
        for (int i = 0; i < net.GetNodeCount(); i++)
        {
            Vector2 position = net.GetNodePosition(i);

            Handles.color = Color.white;
            Handles.DrawSolidDisc(position, Vector3.forward, NodeRadius);

            // hide UI far away from the mouse
            if (Vector2.Distance(mousePosition, position) > EditRange) continue;

            // if a node is being dragged it must be drawn first
            // this allows other nodes to be drawn over then, and thus the other node can be clicked to merge
            if (!IsDragging(i))
                DrawDragHandle(i, mousePosition);

            // hide the buttons while dragging
            if (IsDragging()) continue;

            Handles.color = Color.green;
            if (DrawButton(position + new Vector2(-ButtonSpacing / 2, -NodeRadius * 2), "+", true))
            {
                BeginDrag(net.ForkNode(i, mousePosition));
                EditorUtility.SetDirty(net);
            }

            Handles.color = Color.red;
            if (DrawButton(position + new Vector2(ButtonSpacing / 2, -NodeRadius * 2), "-", true))
            {
                net.DeleteNode(i);
                EditorUtility.SetDirty(net);
                i--;
            }
        }
    }

    /**
     * Draws a button that can be clicked in the editor. Returns true if just clicked.
     */
    private bool DrawButton(Vector2 position, string text, bool nodeButton)
    {
        DrawText(position, text);
        if (nodeButton)
        {
            return Handles.Button(position, Quaternion.identity, ButtonSize, ButtonSize, Handles.RectangleHandleCap);
        }
        else
        {
            return Handles.Button(position, Quaternion.identity, ButtonSize, ButtonSize, Handles.CircleHandleCap);
        }
    }

    /**
     * Draws centered text that can be seen in the editor.
     */
    private void DrawText(Vector2 position, string text)
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16
        };

        Handles.Label(position, text, style);
    }

    /**
     * Draws a handle for dragging path nodes, and handles their interaction.
     */
    private void DrawDragHandle(int index, Vector2 mousePosition)
    {
        PathNetwork net = target as PathNetwork;
        Vector2 position = net.GetNodePosition(index);

        Handles.color = Handles.xAxisColor;
        Handles.DrawLine(position, position + Vector2.up * (NodeRadius * 3));
        Handles.color = Handles.yAxisColor;
        Handles.DrawLine(position, position + Vector2.right * (NodeRadius * 3));

        Handles.color = Handles.zAxisColor;
        if (Handles.Button(position, Quaternion.identity, NodeRadius, NodeRadius, Handles.RectangleHandleCap))
        {
            if (IsDragging(index))
            {
                FinishDrag();
            }
            else if (!IsDragging())
            {
                BeginDrag(index);
            }
            else
            {
                net.MergeNode(index, draggingIndex);
                EditorUtility.SetDirty(net);
                FinishDrag();
            }
        }

        if (IsDragging(index))
        {
            net.MoveNode(index, mousePosition);
            EditorUtility.SetDirty(net);
        }
    }

    /**
     * Finds the position of the cursor in world space.
     */
    private static Vector2 GetMouseWorldPosition()
    {
        Vector3 worldPosition = Event.current.mousePosition;

        Ray ray = HandleUtility.GUIPointToWorldRay(worldPosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        plane.Raycast(ray, out float dist);
        return ray.GetPoint(dist);
    }

    /**
     * Checks if a node is being dragged.
     */
    private bool IsDragging()
    {
        return draggingIndex != -1;
    }

    /**
     * Checks if a particular node is being dragged by its index.
     */
    private bool IsDragging(int index)
    {
        return draggingIndex == index;
    }

    /**
     * Stops dragging any nodes.
     */
    private void FinishDrag()
    {
        draggingIndex = -1;
    }

    /**
     * Starts dragging a node of a given index.
     */
    private void BeginDrag(int node)
    {
        draggingIndex = node;
    }
}