using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Calculates the shortest path between points in the path network using A*.
 * https://en.wikipedia.org/wiki/A*_search_algorithm
 */
public static class AStarPathfinder
{
    /**
     * Calculate a path between any two positions in the world at a given time.
     * Returned path will begin with the nearest path section from a start position. This means the very first node is going away from the goal.
     * Returned path will end with the nearest path section to the goal position. This means the very last node is going past the goal.
     * Allow returns the two points on the network nearest to the start and end positions.
     *
     *                   3 ---- 4 ---N- 5
     *       S          /            G
     *  0 ---N- 1 ---- 2
     *
     *  0-5 = Path, S = startPosition, G = goalPosition, N = NearestStart and NearestGoal
     */
    public static (List<int> Path, Vector2 NearestStart, Vector2 NearestGoal) CalculatePathBetweenPositions(
        Vector2 startPosition, Vector2 goalPosition, bool isFuture)
    {
        PathNetwork net = PathNetwork.Instance;

        (Vector2 nearestStartPoint, int startPath) =
            PathNetwork.Instance.NearestPointOnPaths(startPosition, isFuture);

        float startADistance = Vector2.Distance(net.GetPathPositionA(startPath), nearestStartPoint);
        float startBDistance = Vector2.Distance(net.GetPathPositionB(startPath), nearestStartPoint);

        int start, altStart;
        if (startADistance < startBDistance)
        {
            start = net.GetPathNodeA(startPath);
            altStart = net.GetPathNodeB(startPath);
        }
        else
        {
            start = net.GetPathNodeB(startPath);
            altStart = net.GetPathNodeA(startPath);
        }

        (Vector2 nearestGoalPoint, int goalPath) = PathNetwork.Instance.NearestPointOnPaths(goalPosition, isFuture);

        float goalADistance = Vector2.Distance(net.GetPathPositionA(goalPath), nearestGoalPoint);
        float goalBDistance = Vector2.Distance(net.GetPathPositionB(goalPath), nearestGoalPoint);

        int goal, altGoal;
        if (goalADistance < goalBDistance)
        {
            goal = net.GetPathNodeA(goalPath);
            altGoal = net.GetPathNodeB(goalPath);
        }
        else
        {
            goal = net.GetPathNodeB(goalPath);
            altGoal = net.GetPathNodeA(goalPath);
        }

        List<int> path = AStarPathfinder.CalculatePathBetweenNodes(start, goal, isFuture);
        if (path == null)
            return (null, nearestStartPoint, nearestGoalPoint);

        // make sure start position is in between start and altStart
        if (!path.Contains(altStart))
        {
            path.Insert(0, altStart);
        }

        // make sure end position is in between goal and altGoal
        if (!path.Contains(altGoal))
        {
            path.Add(altGoal);
        }

        return (path, nearestStartPoint, nearestGoalPoint);
    }

    /**
     * Calculates the shortest path from a start node to goal node along the path network at a given time.
     * Returns a path containing both the start and end nodes.
     */
    public static List<int> CalculatePathBetweenNodes(int start, int goal, bool isFuture)
    {
        MinHeap<AStarNode> openSet = new MinHeap<AStarNode>();
        HashSet<int> closedSet = new HashSet<int>();
        Dictionary<int, float> gScores = new Dictionary<int, float>();
        Dictionary<int, AStarNode> parents = new Dictionary<int, AStarNode>();

        AStarNode startNode = new AStarNode(null, start, Heuristic(start, goal));
        openSet.Push(startNode);

        while (openSet.Count() > 0)
        {
            AStarNode currentNode = openSet.Pop();

            if (currentNode.PathNode == goal)
                return ReconstructPath(currentNode);

            closedSet.Add(currentNode.PathNode);

            // check each neighbour
            foreach (int neighbour in currentNode.Neighbours(isFuture))
            {
                if (closedSet.Contains(neighbour)) continue;

                AStarNode neighbourNode = new AStarNode(currentNode, neighbour, Heuristic(neighbour, goal));

                if (gScores.TryGetValue(neighbour, out var score))
                {
                    if (neighbourNode.GScore < score)
                    {
                        // reconstruct what the old neighbour node must be like
                        AStarNode oldNeighbourNode = new AStarNode(parents[neighbour], neighbour, neighbourNode.HScore);
                        // replace it with a better parent
                        openSet.Replace(oldNeighbourNode, neighbourNode);
                    }
                    else
                        // node already visited with a better (or equal) path
                        continue;
                }

                openSet.Push(neighbourNode);
                gScores[neighbour] = neighbourNode.GScore;
                parents[neighbour] = neighbourNode.Parent;
            }
        }

        return null;
    }

    /**
     * Estimates how close the current node is to completing a path to the goal node.
     */
    private static float Heuristic(int current, int goal)
    {
        Vector2 positionCurrent = PathNetwork.Instance.GetNodePosition(current);
        Vector2 positionGoal = PathNetwork.Instance.GetNodePosition(goal);
        return Vector2.Distance(positionCurrent, positionGoal);
    }

    /**
     * Rebuilds the nodes needed to make a path.
     */
    private static List<int> ReconstructPath(AStarNode currentNode)
    {
        List<int> backwardsPath = new List<int>();

        while (currentNode != null)
        {
            backwardsPath.Add(currentNode.PathNode);
            currentNode = currentNode.Parent;
        }

        backwardsPath.Reverse();
        return backwardsPath;
    }

    /**
     * Check that a path created by the pathfinder hasn't been destroyed by changes.
     */
    public static bool CheckPathStillValid(List<int> path, bool isFuture)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            if (!PathNetwork.Instance.AreNodesConnected(path[i], path[i + 1], isFuture))
                return false;
        }

        return true;
    }

    /**
     * Stores the scores of a node in the open set/
     */
    private class AStarNode : IComparable<AStarNode>
    {
        public readonly int PathNode;
        public readonly AStarNode Parent;
        public readonly float GScore;
        public readonly float HScore;
        private readonly float fScore;

        public AStarNode(AStarNode parent, int pathNode, float hScore)
        {
            this.Parent = parent;
            this.PathNode = pathNode;
            this.HScore = hScore;

            if (parent == null)
                this.GScore = 0;
            else
                this.GScore = parent.GScore + Vector2.Distance(Position(), parent.Position());

            this.fScore = GScore + HScore;
        }

        public int CompareTo(AStarNode other)
        {
            return fScore.CompareTo(other.fScore);
        }

        /**
         * Get the world space position in the path network.
         */
        public Vector2 Position()
        {
            return PathNetwork.Instance.GetNodePosition(PathNode);
        }

        /**
         * Get neighbours nodes that can be traversed to at a given time.
         */
        public List<int> Neighbours(bool isFuture)
        {
            return PathNetwork.Instance.GetNodeNeighbours(PathNode, isFuture);
        }
    }
}