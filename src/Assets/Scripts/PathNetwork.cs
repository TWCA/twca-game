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

        // move merged node to midpoint
        nodes[mergedNode] = (nodes[mergedNode] + nodes[discardedNode]) / 2;

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

    public Path getPath(int path)
    {
        return paths[path];
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
