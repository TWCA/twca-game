using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathNetwork : MonoBehaviour
{
    [SerializeField] private List<PathNode> nodes;
    [SerializeField] private List<Path> paths;

    public static PathNetwork Instance { get; private set; }

    PathNetwork()
    {
        nodes = new List<PathNode>();
        paths = new List<Path>();
    }

    private void Awake()
    {
        Instance = this;
    }

    public (float Distance, int Node) NearestNode(Vector2 position)
    {
        int nearestNode = -1;
        float nearestDistance = Single.PositiveInfinity;

        for (int i = 0; i < nodes.Count; i++)
        {
            float distance = Vector2.Distance(position, nodes[i].position);

            if (distance < nearestDistance)
            {
                nearestNode = i;
                nearestDistance = distance;
            }
        }

        return (nearestDistance, nearestNode);
    }

    public (Vector2 Position, int Path) NearestPointOnPaths(Vector2 position, bool isFuture)
    {
        int nearestPath = -1;
        Vector2 nearestPointOverall = Vector2.zero;
        float nearestDistance = Single.PositiveInfinity;

        for (int i = 0; i < paths.Count; i++)
        {
            // skip paths from another time
            if (!paths[i].Traversable(isFuture)) continue;

            Vector2 nearestPoint = NearestPointOnPath(i, position);
            float distance = Vector2.Distance(position, nearestPoint);

            if (distance < nearestDistance)
            {
                nearestPath = i;
                nearestPointOverall = nearestPoint;
                nearestDistance = distance;
            }
        }

        return (nearestPointOverall, nearestPath);
    }

    public Vector2 NearestUnclampedPointOnPath(int path, Vector2 point)
    {
        return NearestPointOnPath(path, point, false);
    }

    public Vector2 NearestPointOnPath(int path, Vector2 point)
    {
        return NearestPointOnPath(path, point, true);
    }

    public Vector2 NearestPointOnPath(int path, Vector2 point, bool clamp)
    {
        Vector2 positionA = GetPathPositionA(path);
        Vector2 positionB = GetPathPositionB(path);
        Vector2 pathDirection = (positionB - positionA).normalized;
        float pathLength = (positionB - positionA).magnitude;

        float distanceAlong = Vector3.Dot(point - positionA, pathDirection);

        if (clamp)
            distanceAlong = Mathf.Clamp(distanceAlong, 0, pathLength);

        return positionA + pathDirection * distanceAlong;
    }

    public void MoveNode(int node, Vector2 position)
    {
        nodes[node].position = position;
    }

    public int CreateNode(Vector2 position)
    {
        nodes.Add(new PathNode(position));
        return nodes.Count - 1;
    }

    /** Returns the index of the of newly created node */
    public int ForkNode(int node, Vector2 position)
    {
        paths.Add(new Path(node, nodes.Count));

        nodes.Add(new PathNode(position, node));
        foreach (List<int> neighbours in nodes[node].PastAndFutureNeighbourhoods)
        {
            neighbours.Add(nodes.Count - 1);
        }

        return nodes.Count - 1;
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

            if (path.nodeA == discardedNode)
            {
                path.nodeA = mergedNode; // update path to connect to merged node

                // add mergedNode to neighbours list in it's neighbours
                foreach (List<int> neighbours in nodes[path.nodeB].PastAndFutureNeighbourhoods)
                {
                    if (neighbours.Contains(discardedNode))
                    {
                        neighbours.Remove(discardedNode);
                        if (!neighbours.Contains(mergedNode))
                            neighbours.Add(mergedNode);
                    }
                }
            }
            else if (path.nodeA > discardedNode)
                path.nodeA--; // update path to respect re-ording of nodes

            if (path.nodeB == discardedNode)
            {
                path.nodeB = mergedNode; // update path to connect to merged node
                
                // add mergedNode to neighbours list in it's neighbours
                foreach (List<int> neighbours in nodes[path.nodeA].PastAndFutureNeighbourhoods)
                {
                    if (neighbours.Contains(discardedNode))
                    {
                        neighbours.Remove(discardedNode);
                        if (!neighbours.Contains(mergedNode))
                            neighbours.Add(mergedNode);
                    }
                }
            }
            else if (path.nodeB > discardedNode)
                path.nodeB--; // update path to respect re-ording of nodes
        }

        // track all the nodes connected to the merged node
        // thus if we find repeats we know there are two identical paths
        HashSet<int> connectedNodes = new HashSet<int>();

        foreach (List<int> neighbours in nodes[mergedNode].PastAndFutureNeighbourhoods)
        {
            neighbours.Clear();
        }

        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];

            if (path.nodeA == path.nodeB)
            {
                // remove paths in between merged nodes
                DeletePath(i);
                i--;
            }
            else if (path.nodeA == mergedNode)
            {
                if (connectedNodes.Contains(path.nodeB))
                {
                    // identical path, remove the copy
                    DeletePath(i);
                    i--;
                }
                else
                {
                    connectedNodes.Add(path.nodeB);
                    
                    foreach (List<int> neighbours in nodes[mergedNode].PastAndFutureNeighbourhoods)
                    {
                        neighbours.Add(path.nodeB);
                    }
                }
            }
            else if (path.nodeB == mergedNode)
            {
                if (connectedNodes.Contains(path.nodeA))
                {
                    // identical path, remove the copy
                    DeletePath(i);
                    i--;
                }
                else
                {
                    connectedNodes.Add(path.nodeA);
                    
                    foreach (List<int> neighbours in nodes[mergedNode].PastAndFutureNeighbourhoods)
                    {
                        neighbours.Add(path.nodeA);
                    }
                }
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            foreach (List<int> neighbours in nodes[i].PastAndFutureNeighbourhoods)
            {
                for (int j = 0; j < neighbours.Count; j++)
                {
                    if (neighbours[j] > discardedNode)
                        // update neighbours to respect re-ording of nodes
                        neighbours[j]--;
                }
            }
        }
    }

    public void DeleteNode(int node)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];
            if (path.nodeA == node || path.nodeB == node)
            {
                DeletePath(i);
                i--;
            }
            else
            {
                // update path to respect re-ording of nodes
                if (path.nodeA > node)
                    path.nodeA--;

                if (path.nodeB > node)
                    path.nodeB--;
            }
        }

        // update neighbours to respect re-ording of nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            foreach (List<int> neighbours in nodes[i].PastAndFutureNeighbourhoods)
            {
                for (int j = 0; j < neighbours.Count; j++)
                {
                    if (neighbours[j] > node)
                        neighbours[j]--;
                }
            }
        }

        nodes.RemoveAt(node);
    }

    /** Returns the index of the of newly created midpoint node */
    public int BreakPath(int path)
    {
        Path existingPath = paths[path];
        Vector2 midpointPosition = (GetPathPositionB(path) + GetPathPositionB(path)) / 2;

        foreach (List<int> neighbours in nodes[existingPath.nodeA].PastAndFutureNeighbourhoods)
        {
            neighbours.Remove(existingPath.nodeB);
            neighbours.Add(nodes.Count);
        }

        foreach (List<int> neighbours in nodes[existingPath.nodeB].PastAndFutureNeighbourhoods)
        {
            neighbours.Remove(existingPath.nodeA);
            neighbours.Add(nodes.Count);
        }

        nodes.Add(new PathNode(midpointPosition, existingPath.nodeA, existingPath.nodeB));

        paths.Add(new Path(nodes.Count - 1, existingPath.nodeB));
        existingPath.nodeB = nodes.Count - 1;

        return nodes.Count - 1;
    }

    public void DeletePath(int path)
    {
        foreach (List<int> neighbours in nodes[paths[path].nodeA].PastAndFutureNeighbourhoods)
            neighbours.Remove(paths[path].nodeB);

        foreach (List<int> neighbours in nodes[paths[path].nodeB].PastAndFutureNeighbourhoods)
            neighbours.Remove(paths[path].nodeA);

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

    public Vector2 GetNodePosition(int node)
    {
        return nodes[node].position;
    }

    public bool AreNodesConnected(int nodeA, int nodeB, bool isFuture)
    {
        if (isFuture)
            return nodes[nodeA].futureNeighbours.Contains(nodeB);
        else
            return nodes[nodeA].pastNeighbours.Contains(nodeB);
    }

    public List<int> GetNodeNeighbours(int node, bool isFuture)
    {
        if (isFuture)
            return nodes[node].futureNeighbours;
        else
            return nodes[node].pastNeighbours;
    }

    public (Vector2 Start, Vector2 End) PathPointsGoingDirection(int path, Vector2 direction)
    {
        Vector2 aPosition = GetPathPositionA(path);
        Vector2 bPosition = GetPathPositionB(path);

        if (Vector3.Dot(bPosition - aPosition, direction) > 0)
            return (aPosition, bPosition);
        else
            return (bPosition, aPosition);
    }

    public (Vector2 Start, Vector2 End) PathPointsComingFrom(int path, Vector2 position)
    {
        Vector2 aPosition = GetPathPositionA(path);
        Vector2 bPosition = GetPathPositionB(path);

        if (Vector2.Distance(aPosition, position) < Vector2.Distance(bPosition, position))
            return (aPosition, bPosition);
        else
            return (bPosition, aPosition);
    }

    public Vector2 GetPathPositionA(int path)
    {
        return nodes[paths[path].nodeA].position;
    }

    public Vector2 GetPathPositionB(int path)
    {
        return nodes[paths[path].nodeB].position;
    }

    public int GetPathNodeA(int path)
    {
        return paths[path].nodeA;
    }

    public int GetPathNodeB(int path)
    {
        return paths[path].nodeB;
    }


    /**  get all connected paths from nodeA */
    public List<int> GetPathConnectionsA(int path, bool isFuture)
    {
        return GetPathConnections(path, paths[path].nodeA, isFuture);
    }

    /** get all connected paths from nodeB */
    public List<int> GetPathConnectionsB(int path, bool isFuture)
    {
        return GetPathConnections(path, paths[path].nodeB, isFuture);
    }

    private List<int> GetPathConnections(int path, int node, bool isFuture)
    {
        List<int> connectedPaths = new List<int>();

        for (int i = 0; i < paths.Count; i++)
        {
            if (i == path) continue;

            // skip paths from another time
            if (!paths[i].Traversable(isFuture)) continue;

            if (paths[i].nodeA == node ||
                paths[i].nodeB == node)
            {
                connectedPaths.Add(i);
            }
        }

        return connectedPaths;
    }

    public bool GetPathPastTraversable(int path)
    {
        return paths[path].pastTraversable;
    }

    public bool GetPathFutureTraversable(int path)
    {
        return paths[path].futureTraversable;
    }

    public string GetPathName(int path)
    {
        return paths[path].name;
    }


    [System.Serializable]
    private class Path
    {
        [HideInInspector] public int nodeA;
        [HideInInspector] public int nodeB;
        public string name;
        public bool pastTraversable = true;
        public bool futureTraversable = true;

        public Path(int nodeA, int nodeB)
        {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
        }

        public bool Traversable(bool isFuture)
        {
            if (isFuture)
            {
                return futureTraversable;
            }
            else
            {
                return pastTraversable;
            }
        }
    }

    [System.Serializable]
    private class PathNode
    {
        public Vector2 position;
        [HideInInspector] public List<int> pastNeighbours;
        [HideInInspector] public List<int> futureNeighbours;

        public List<int>[] PastAndFutureNeighbourhoods
        {
            get { return new[] { pastNeighbours, futureNeighbours }; }
        }

        public PathNode(Vector2 position, params int[] neighbours)
        {
            this.position = position;
            this.pastNeighbours = neighbours.ToList();
            this.futureNeighbours = neighbours.ToList();
        }
    }
}