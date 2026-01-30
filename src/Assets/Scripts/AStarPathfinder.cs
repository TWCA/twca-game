using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<int> CalculatePath(int start, int goal, bool isFuture)
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
        
            // check each neighbor
            foreach (int neighbour in currentNode.Neighbours(isFuture))
            {
                if (closedSet.Contains(neighbour)) continue;
                
                AStarNode neighbourNode = new AStarNode(currentNode, neighbour, Heuristic(neighbour, goal));
                
                if (gScores.ContainsKey(neighbour))
                {
                    if (neighbourNode.GScore < gScores[neighbour])
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

    private static float Heuristic(int current, int goal)
    {
        Vector2 positionCurrent = PathNetwork.Instance.GetNodePosition(current);
        Vector2 positionGoal = PathNetwork.Instance.GetNodePosition(goal);
        return Vector2.Distance(positionCurrent, positionGoal);
    }

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
}

public class AStarNode : IComparable<AStarNode>
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
    
    public Vector2 Position()
    {
        return PathNetwork.Instance.GetNodePosition(PathNode);
    }

    public List<int> Neighbours(bool isFuture)
    {
        return PathNetwork.Instance.GetNodeNeighbours(PathNode, isFuture);
    }
}