using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public class PathNetwork : MonoBehaviour
{
    [SerializeField] private List<Vector2> nodes;
    [SerializeField] private List<Path> paths;

    public static PathNetwork Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MoveNode(int node, Vector2 position)
    {
        nodes[node] = position;
    }

    public int CreateNode(Vector2 position)
    {
        nodes.Add(position);
        return nodes.Count - 1;
    }

    /** Returns the index of the of newly created node */
    public int ForkNode(int node, Vector2 position)
    {
        nodes.Add(position);
        paths.Add(new Path(node, nodes.Count - 1));

        return nodes.Count -1 ;
    }

    public void MergeNode(int mergedNode, int discardedNode)
    {
        // it's faster to remove the later index, swap if needed
        if (mergedNode > discardedNode)
            (mergedNode, discardedNode) = (discardedNode, mergedNode);

        // remove extra node
        nodes.RemoveAt(discardedNode);

        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];

            if (path.start == discardedNode)
                path.start = mergedNode; // update path to connect to merged node
            else if (path.start > discardedNode)
                path.start--; // update path to respect re-ording of nodes

            if (path.end == discardedNode)
                path.end = mergedNode; // update path to connect to merged node
            else if (path.end > discardedNode)
                path.end--; // update path to respect re-ording of nodes
        }

        // track all the nodes connected to the merged node
        // thus if we find repeats we know there are two identical paths
        HashSet<int> connectedNodes = new HashSet<int>();

        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];

            if (path.start == path.end)
            {
                // remove paths in between merged nodes
                paths.RemoveAt(i);
                i--;
            }
            else if (path.start == mergedNode)
            {
                if (connectedNodes.Contains(path.end))
                {
                    // identical path, remove the copy
                    paths.RemoveAt(i);
                    i--;
                }
                else
                {
                    connectedNodes.Add(path.end);
                }
            }
            else if (path.end == mergedNode)
            {
                if (connectedNodes.Contains(path.start))
                {
                    // identical path, remove the copy
                    paths.RemoveAt(i);
                    i--;
                }
                else
                {
                    connectedNodes.Add(path.start);
                }
            }
        }
    }

    public void DeleteNode(int node)
    {
        nodes.RemoveAt(node);

        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];
            if (path.start == node || path.end == node)
            {
                paths.RemoveAt(i);
                i--;
            }
            else
            {
                if (path.start > node)
                    path.start--;

                if (path.end > node)
                    path.end--;
            }
        }
    }

    /** Returns the index of the of newly created midpoint node */
    public int BreakPath(int path)
    {
        int start = paths[path].start;
        int end = paths[path].end;

        Vector2 midpointPosition = (nodes[start] + nodes[end]) / 2;

        nodes.Add(midpointPosition);

        paths[path].end = nodes.Count - 1;
        paths.Add(new Path(nodes.Count - 1, end));

        return nodes.Count - 1;
    }

    public void DeletePath(int path)
    {
        paths.RemoveAt(path);
    }

    public int GetNodeCount()
    {
        return nodes.Count;
    }

    public int GetPathCount()
    {
        return paths.Count;
    }

    public Vector2 GetNode(int node)
    {
        return nodes[node];
    }

    public int GetPathStart(int path)
    {
        return paths[path].start;
    }
    
    public int GetPathEnd(int path)
    {
        return paths[path].end;
    }
    
    public PathTimespan GetPathTimespan(int path)
    {
        return paths[path].timespan;
    }
    
    public void SetPathTimespan(int path, PathTimespan timespan)
    {
        paths[path].timespan = timespan;
    }
}

public enum PathTimespan
{
    Both,
    Past,
    Future
}

[System.Serializable]
public class Path
{
    public Path(int start, int end)
    {
        this.start = start;
        this.end = end;
    }

    public int start;
    public int end;
    public PathTimespan timespan = PathTimespan.Both;
}


[CustomEditor(typeof(PathNetwork))]
public class PathNetworkEditor : Editor
{
    private const float NodeRadius = 0.2f;
    private const float ButtonSize = 0.18f;
    private const float EditRange = 2.0f;

    private int draggingIndex = -1;

    public void OnSceneGUI()
    {
        PathNetwork net = target as PathNetwork;
        if (net.GetNodeCount() == 0)
        {
            net.CreateNode(Vector2.zero);
            EditorUtility.SetDirty(net);
        }

        Vector2 mousePosition = MouseWorldPosition();
        DrawPaths(mousePosition);
        DrawNodes(mousePosition);
    }

    private void DrawPaths(Vector2 mousePosition)
    {
        PathNetwork net = target as PathNetwork;

        for(int i = 0; i < net.GetPathCount(); i++)
        {
            Vector2 startPosition = net.GetNode(net.GetPathStart(i));
            Vector2 endPosition = net.GetNode(net.GetPathEnd(i));
            Vector2 midpoint = (startPosition + endPosition) / 2;

            string timeLabel = "uninitialized";
            PathTimespan timespan = net.GetPathTimespan(i);
            
            switch (timespan)
            {
                case PathTimespan.Both:
                    timeLabel = "b";
                    Handles.color = Color.white;
                    Handles.DrawLine(startPosition, endPosition);
                    break;
                case PathTimespan.Past:
                    timeLabel = "p";
                    Handles.color = Color.yellow;
                    Handles.DrawDottedLine(startPosition, endPosition, 4.0f);
                    break;
                case PathTimespan.Future:
                    timeLabel = "f";
                    Handles.color = Color.cyan;
                    Handles.DrawDottedLine(startPosition, endPosition, 4.0f);
                    break;
            }

            // hide UI far away from the mouse
            if (Vector2.Distance(mousePosition, midpoint) > EditRange) continue;
            // hide some of the buttons while dragging
            if (IsDragging()) continue;

            // keeps color from path line
            if (DrawButton(midpoint + new Vector2(0, -0.4f), timeLabel, false))
            {
                if (timespan == PathTimespan.Both)
                    net.SetPathTimespan(i, PathTimespan.Past);
                else if (timespan == PathTimespan.Past)
                    net.SetPathTimespan(i, PathTimespan.Future);
                else if (timespan == PathTimespan.Future)
                    net.SetPathTimespan(i, PathTimespan.Both);
                
                EditorUtility.SetDirty(net);
            }
            
            Handles.color = Color.green;
            if (DrawButton(midpoint + new Vector2(-0.4f, -0.4f), "+", false))
            {
                BeginDrag(net.BreakPath(i));
                EditorUtility.SetDirty(net);
            }

            Handles.color = Color.red;
            if (DrawButton(midpoint + new Vector2(0.4f, -0.4f), "-", false))
            {
                net.DeletePath(i);
                EditorUtility.SetDirty(net);
                i--;
            }
        }
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
            Vector2 position = net.GetNode(i);

            Handles.color = Color.white;
            Handles.DrawSolidDisc(position, Vector3.forward, NodeRadius);

            // hide UI far away from the mouse
            if (Vector2.Distance(mousePosition, position) > EditRange) continue;

            // if a node is being dragged it must be drawn first
            // this allows other nodes to be drawn over then, and thus the other node can be clicked to merge
            if (!IsDragging(i))
                DrawDragHandle(i, mousePosition);

            // hide some of the buttons while dragging
            if (IsDragging()) continue;

            Handles.color = Color.green;
            if (DrawButton(position + new Vector2(-0.2f, -0.4f), "+", true))
            {
                BeginDrag(net.ForkNode(i, mousePosition));
                EditorUtility.SetDirty(net);
            }

            Handles.color = Color.red;
            if (DrawButton(position + new Vector2(0.2f, -0.4f), "-", true))
            {
                net.DeleteNode(i);
                EditorUtility.SetDirty(net);
                i--;
            }
        }
    }

    private bool DrawButton(Vector2 position, string label, bool nodeButton)
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 16;

        Handles.Label(position, label, style);
        if (nodeButton)
        {
            return Handles.Button(position, Quaternion.identity, ButtonSize, ButtonSize, Handles.RectangleHandleCap);
        }
        else
        {
            return Handles.Button(position, Quaternion.identity, ButtonSize, ButtonSize, Handles.CircleHandleCap);
        }
    }

    private void DrawDragHandle(int index, Vector2 mousePosition)
    {
        PathNetwork net = target as PathNetwork;
        Vector2 position = net.GetNode(index);

        Handles.color = Handles.xAxisColor;
        Handles.DrawLine(position, position + Vector2.up * (NodeRadius * 3));
        Handles.color = Handles.yAxisColor;
        Handles.DrawLine(position, position + Vector2.right * (NodeRadius * 3));

        Handles.color = Handles.zAxisColor;
        if (Handles.Button(position, Quaternion.identity, NodeRadius, NodeRadius, Handles.RectangleHandleCap))
        {
            if (IsDragging(index))
            {
                EndDrag();
            }
            else if (!IsDragging())
            {
               BeginDrag(index);
            }
            else
            {
                net.MergeNode(index, draggingIndex);
                EditorUtility.SetDirty(net);
                EndDrag();
            }
        }

        if (IsDragging(index))
        {
            net.MoveNode(index, mousePosition);
            EditorUtility.SetDirty(net);
        }
    }

    private Vector2 MouseWorldPosition()
    {
        Vector3 worldPosition = Event.current.mousePosition;

        Ray ray = HandleUtility.GUIPointToWorldRay(worldPosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        plane.Raycast(ray, out float dist);
        return ray.GetPoint(dist);
    }

    private bool IsDragging()
    {
        return draggingIndex != -1;
    }

    private bool IsDragging(int index)
    {
        return draggingIndex == index;
    }

    private void EndDrag()
    {
        draggingIndex = -1;
    }

    private void BeginDrag(int node)
    {
        draggingIndex = node;
    }
}
