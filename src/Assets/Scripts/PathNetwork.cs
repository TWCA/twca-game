using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Singleton that maintains a list of nodes, with paths in-between them. These paths defined where the player can walk.
 * Contains various methods to manipulate and query the path network. Nodes and paths are referred to by their index.
 * Indexes will be rearranged if any node/path is removed! Any indexes returned previously will be invalidated if nodes are removed!
 * This data is frankly a mess due to different constrains and premature optimization. My bad chat.
 */
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

    /**
     * Find the nearest path node from a position
     */
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

    /**
     * Find the nearest point along any path in the network from a position and time
     */
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

    /**
     * Find the nearest point on a single path from a position (but any time)
     *
     * Setting clamp to false will find the closest point to an infinite line going through the path.
     * By default, the nearest point is clamped within the length of the path.
     */
    public Vector2 NearestPointOnPath(int path, Vector2 point, bool clamp = true)
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

    /**
     * Changes the position of a path node
     */
    public void MoveNode(int node, Vector2 position)
    {
        nodes[node].position = position;
    }

    public int CreateNode(Vector2 position)
    {
        nodes.Add(new PathNode(position));
        return nodes.Count - 1;
    }

    /**
     * Creates a new node at a position connected to an existing node. Returns the index of the of newly created node.
     */
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

    /**
     * Merges two nodes into one, keeps all paths connected maintained. Returns the index of the merged node.
     * <br/> <br/>
     * <b>This will invalidate all previously returned node and path indexes! This will break A* paths.</b>
     */
    public int MergeNode(int nodeA, int nodeB)
    {
        // it's faster to remove the later index, swap if needed
        int mergedNode, discardedNode;
        if (nodeA < nodeB)
        {
            mergedNode = nodeA;
            discardedNode = nodeB;
        }
        else
        {
            mergedNode = nodeB;
            discardedNode = nodeA;
        }

        // remove extra node
        nodes.RemoveAt(discardedNode);

        foreach (var path in paths)
        {
            if (path.nodeA == discardedNode)
            {
                path.nodeA = mergedNode; // update path to connect to merged node

                // add mergedNode to neighbours list in its neighbours
                foreach (List<int> neighbours in nodes[path.nodeB].PastAndFutureNeighbourhoods)
                {
                    if (!neighbours.Contains(discardedNode)) continue;
                    
                    neighbours.Remove(discardedNode);
                    if (!neighbours.Contains(mergedNode))
                        neighbours.Add(mergedNode);
                }
            }
            else if (path.nodeA > discardedNode)
                path.nodeA--; // update path to respect reordering of nodes

            if (path.nodeB == discardedNode)
            {
                path.nodeB = mergedNode; // update path to connect to merged node

                // add mergedNode to neighbours list in its neighbours
                foreach (List<int> neighbours in nodes[path.nodeA].PastAndFutureNeighbourhoods)
                {
                    if (!neighbours.Contains(discardedNode)) continue;
                    
                    neighbours.Remove(discardedNode);
                    if (!neighbours.Contains(mergedNode))
                        neighbours.Add(mergedNode);
                }
            }
            else if (path.nodeB > discardedNode)
                path.nodeB--; // update path to respect reordering of nodes
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
                ErasePath(i);
                i--;
            }
            else if (path.nodeA == mergedNode)
            {
                if (!connectedNodes.Add(path.nodeB))
                {
                    // identical path already exists, remove the copy
                    ErasePath(i);
                    i--;
                }
                else
                {
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
                    // identical path already exists, remove the copy
                    ErasePath(i);
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

        foreach (var t in nodes)
        {
            foreach (List<int> neighbours in t.PastAndFutureNeighbourhoods)
            {
                for (int j = 0; j < neighbours.Count; j++)
                {
                    if (neighbours[j] > discardedNode)
                        // update neighbours to respect reordering of nodes
                        neighbours[j]--;
                }
            }
        }

        return mergedNode;
    }


    /**
     * Removes any path node that is not connected to a path. Returns the number of nodes erased.
     * <br/> <br/>
     * <b>This will invalidate all previously returned node indexes! This will break A* paths.</b>
     */
    public int EraseOrphanNodes()
    {
        HashSet<int> connectedNodes = new HashSet<int>();
        
        for (int i = 0; i < paths.Count; i++)
        {
            connectedNodes.Add(paths[i].nodeA);
            connectedNodes.Add(paths[i].nodeB);
        }

        int nodesErased = 0;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (!connectedNodes.Contains(i + nodesErased))
            {
                EraseNode(i);
                i--;
                nodesErased++;
            }
        }

        return nodesErased;
    }
    
    /**
     * Removes a path node, and all connected paths.
     * <br/> <br/>
     * <b>This will invalidate all previously returned node and path indexes! This will break A* paths.</b>
     */
    public void EraseNode(int node)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];
            if (path.nodeA == node || path.nodeB == node)
            {
                ErasePath(i);
                i--;
            }
            else
            {
                // update path to respect reordering of nodes
                if (path.nodeA > node)
                    path.nodeA--;

                if (path.nodeB > node)
                    path.nodeB--;
            }
        }

        // update neighbours to respect reordering of nodes
        foreach (var t in nodes)
        {
            foreach (List<int> neighbours in t.PastAndFutureNeighbourhoods)
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

    /**
     * Breaks a path in two, adding a node in between at the midpoint.
     * Returns the index of the of newly created middle node.
     */
    public int BreakPath(int path)
    {
        Vector2 midpoint = (GetPathPositionA(path) + GetPathPositionB(path)) / 2;
        return BreakPath(path, midpoint);
    }

    /**
     * Breaks a path in two, adding a node in between at a given position.
     * Returns the index of the of newly created middle node.
     */
    public int BreakPath(int path, Vector2 middlePosition)
    {
        Path existingPath = paths[path];

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

        nodes.Add(new PathNode(middlePosition, existingPath.nodeA, existingPath.nodeB));

        paths.Add(new Path(nodes.Count - 1, existingPath.nodeB));
        existingPath.nodeB = nodes.Count - 1;

        return nodes.Count - 1;
    }

    /**
     * Deletes a path between to nodes.
     * <br/> <br/>
     * <b>This will invalidate all previously returned path indexes!</b> Node indexes are unaffected.
     */
    public void ErasePath(int path)
    {
        foreach (List<int> neighbours in nodes[paths[path].nodeA].PastAndFutureNeighbourhoods)
            neighbours.Remove(paths[path].nodeB);

        foreach (List<int> neighbours in nodes[paths[path].nodeB].PastAndFutureNeighbourhoods)
            neighbours.Remove(paths[path].nodeA);

        paths.RemoveAt(path);
    }

    /**
     * Gets the numbers of nodes.
     */
    public int GetNodeCount()
    {
        return nodes.Count;
    }

    /**
     * Gets the numbers of paths.
     */
    public int GetPathCount()
    {
        return paths.Count;
    }

    /**
     * Gets the position of a node.
     */
    public Vector2 GetNodePosition(int node)
    {
        return nodes[node].position;
    }

    /**
     * Checks if two nodes are connected by a path at a given time.
     * Returns false if the nodes are the same.
     */
    public bool AreNodesConnected(int nodeA, int nodeB, bool isFuture)
    {
        if (isFuture)
            return nodes[nodeA].futureNeighbours.Contains(nodeB);
        else
            return nodes[nodeA].pastNeighbours.Contains(nodeB);
    }

    /**
     * Gets a list of all nodes connected by paths to a given node at a given time.
     */
    public List<int> GetNodeNeighbours(int node, bool isFuture)
    {
        if (isFuture)
            return nodes[node].futureNeighbours;
        else
            return nodes[node].pastNeighbours;
    }

    /**
     * Gets the positions of the start and end of a path, such that the start is nearer along a given direction.
     */
    public (Vector2 Start, Vector2 End) PathPointsGoingDirection(int path, Vector2 direction)
    {
        Vector2 aPosition = GetPathPositionA(path);
        Vector2 bPosition = GetPathPositionB(path);

        if (Vector3.Dot(bPosition - aPosition, direction) > 0)
            return (aPosition, bPosition);
        else
            return (bPosition, aPosition);
    }

    /**
     * Gets the positions of the start and end of a path, such that the start is nearer to a position.
     * This is different from GetPathPositionA() and GetPathPositionB()
     */
    public (Vector2 Start, Vector2 End) PathPointsComingFrom(int path, Vector2 position)
    {
        Vector2 aPosition = GetPathPositionA(path);
        Vector2 bPosition = GetPathPositionB(path);

        if (Vector2.Distance(aPosition, position) < Vector2.Distance(bPosition, position))
            return (aPosition, bPosition);
        else
            return (bPosition, aPosition);
    }

    /**
     * Gets the position of the A end of a path.
     * Which end is A or B is constant, but otherwise arbitrary.
     */
    public Vector2 GetPathPositionA(int path)
    {
        return nodes[paths[path].nodeA].position;
    }

    /**
     * Gets the position of the B end of a path.
     * Which end is A or B is constant, but otherwise arbitrary.
     */
    public Vector2 GetPathPositionB(int path)
    {
        return nodes[paths[path].nodeB].position;
    }

    /**
     * Get the index of the node at the A end of a path.
     * Which end is A or B is constant, but otherwise arbitrary.
     */
    public int GetPathNodeA(int path)
    {
        return paths[path].nodeA;
    }

    /**
     * Get the index of the node at the B end of a path.
     * Which end is A or B is constant, but otherwise arbitrary.
     */
    public int GetPathNodeB(int path)
    {
        return paths[path].nodeB;
    }


    /**
     * Get all paths connected at the A end of a path, which are traversable at a given time.
     * Which end is A or B is constant, but otherwise arbitrary.
     */
    public List<int> GetPathConnectionsA(int path, bool isFuture)
    {
        return GetPathConnections(path, paths[path].nodeA, isFuture);
    }

    /**
     * Get all paths connected at the B end of a path, which are traversable at a given time.
     * Which end is A or B is constant, but otherwise arbitrary.
     */
    public List<int> GetPathConnectionsB(int path, bool isFuture)
    {
        return GetPathConnections(path, paths[path].nodeB, isFuture);
    }

    /**
     * Get all paths connect to a given node, which are traversable at a given time. Excludes one source path.
     */
    private List<int> GetPathConnections(int source, int node, bool isFuture)
    {
        List<int> connectedPaths = new List<int>();

        for (int i = 0; i < paths.Count; i++)
        {
            if (i == source) continue;

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

    /**
     * Checks if a path is traversable at a given time.
     */
    public bool IsPathTraversableAtTime(int path, bool isFuture)
    {
        return paths[path].Traversable(isFuture);
    }

    /**
     * Gets named path. Returns -1 if not found, or if there are multiple matches.
     * This can be used to label special paths for easy manipulation in puzzles.
     */
    public int GetNamedPath(string pathName)
    {
        List<int> matches = new List<int>();

        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].name == pathName)
                matches.Add(i);
        }

        if (matches.Count == 1)
        {
            return matches[0];
        }
        else if (matches.Count < 1)
        {
            Debug.LogWarning("Found no matches for named path, returning -1");
            return -1;
        }
        else // matches.Count > 1
        {
            Debug.LogWarning("Found multiple matches for named path, returning -1");
            return -1;
        }
    }

    /**
     * Gets the name of a path.
     * This can be used to label special paths for easy manipulation in puzzles.
     */
    public string GetPathName(int path)
    {
        return paths[path].name;
    }

    /**
     * Represents a path between two nodes.
     * Paths may be traversable at some times, but not others.
     * Paths may be named for easy manipulation in puzzles.
     */
    [Serializable]
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

        /**
         * Checks if a path is traversable at a given time.
         */
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

    /**
     * Represents a path network node, defines the position of path ends.
     * Keeps track of neighbour nodes for fast access.
     */
    [Serializable]
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