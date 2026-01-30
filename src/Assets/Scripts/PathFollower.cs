using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    // replace later
    private const bool isFuture = false;

    public float speed = 1f;

    public float intersectionSize = 0.4f;

    /* the max angle in degrees between target walk direction and the path */
    [Range(0.0f, 90.0f)] public float maxPathError = 60f;
    [Range(0.0f, 1.0f)] public float pathLerpRate = 0.9f;

    private List<int> plannedPath = null;
    private Vector2 plannedPosition = Vector2.zero;

    public void FixedUpdate()
    {
        if (!IsPathfinding()) return;

        PathNetwork net = PathNetwork.Instance;

        // check path is still valid
        for (int i = 0; i < plannedPath.Count - 1; i++)
        {
            if (!net.AreNodesConnected(plannedPath[i], plannedPath[i + 1], isFuture))
            {
                if (i == plannedPath.Count - 2)
                    StopPathfinding(); // last section of path broken, goal unreachable
                else
                    if (!PathfindTo(plannedPosition)) // try to path
                    StopPathfinding(); // if failed stop pathfinding
                
                break;
            }
        }
    }

    public void Update()
    {
        if (!IsPathfinding()) return;

        PathNetwork net = PathNetwork.Instance;

        Vector2 targetPosition;
        if (plannedPath.Count > 2)
            targetPosition = net.GetNodePosition(plannedPath[1]);
        else
            targetPosition = plannedPosition;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position.Equals(targetPosition))
        {
            plannedPath.RemoveAt(0);

            // if there are no more edges to follow, stop pathing
            if (plannedPath.Count <= 1)
                StopPathfinding();
        }
    }

    public bool PathfindTo(Vector2 target)
    {
        PathNetwork net = PathNetwork.Instance;

        (Vector2 nearestStartPoint, int startPath) =
            PathNetwork.Instance.NearestPointOnPaths(transform.position, isFuture);

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

        (Vector2 nearestGoalPoint, int goalPath) = PathNetwork.Instance.NearestPointOnPaths(target, isFuture);

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

        List<int> path = AStarPathfinder.CalculatePath(start, goal, isFuture);
        if (path == null) return false;

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

        plannedPath = path;
        plannedPosition = nearestGoalPoint;
        return true;
    }

    public void StopPathfinding()
    {
        plannedPath = null;
        plannedPosition = Vector2.zero;
    }

    public bool IsPathfinding()
    {
        return plannedPath != null;
    }

    public void WalkTowards(Vector2 direction, float delta)
    {
        PathNetwork net = PathNetwork.Instance;

        StopPathfinding();

        Vector2 nearestPoint;
        if (!direction.Equals(Vector2.zero))
        {
            (int path, Vector2 pathEnd) = ChoosePathToWalkOn(direction, isFuture);
            transform.position = Vector2.MoveTowards(transform.position, pathEnd, speed * delta);

            nearestPoint = net.NearestUnclampedPointOnPath(path, transform.position);
        }
        else
        {
            (nearestPoint, _) = net.NearestPointOnPaths(transform.position, isFuture);
        }

        float lerpAmount = 1 - Mathf.Pow(1 - pathLerpRate, delta);
        transform.position = Vector2.Lerp(transform.position, nearestPoint, lerpAmount);
    }

    private (int Path, Vector2 pathEnd) ChoosePathToWalkOn(Vector2 targetDirection, bool isFuture)
    {
        List<int> paths = GetTraversablePaths(isFuture);

        int bestPath = paths[0];
        float bestError = Single.PositiveInfinity;
        Vector2 bestPathEnd = transform.position;

        if (paths.Count == 1)
        {
            bestPath = paths[0];
            (bestError, bestPathEnd) = PathErrorDirectional(paths[0], targetDirection);
        }
        else
        {
            foreach (int path in paths)
            {
                (float error, Vector2 pathEnd) =
                    PathErrorPositional(path, targetDirection);

                if (error < bestError)
                {
                    bestPath = path;
                    bestError = error;
                    bestPathEnd = pathEnd;
                }
            }
        }

        // check if we can even walk
        if (bestError < maxPathError)
            return (bestPath, bestPathEnd);
        else
            return (bestPath, transform.position);
    }

    /** Find how closely this path matches the targetDirection, assuming we can go either way */
    private (float AngleError, Vector2 pathEnd) PathErrorDirectional(int path, Vector2 targetDirection)
    {
        PathNetwork net = PathNetwork.Instance;

        (Vector2 start, Vector2 end) = net.PathPointsGoingDirection(path, targetDirection);
        float angleError = Vector2.Angle(end - start, targetDirection);
        return (angleError, end);
    }

    /** Find how closely this path matches the targetDirection, assuming we must walk from a position */
    private (float AngleError, Vector2 pathEnd) PathErrorPositional(int path, Vector2 targetDirection)
    {
        PathNetwork net = PathNetwork.Instance;

        (Vector2 start, Vector2 end) = net.PathPointsComingFrom(path, transform.position);
        float angleError = Vector2.Angle(end - start, targetDirection);

        return (angleError, end);
    }

    private List<int> GetTraversablePaths(bool isFuture)
    {
        PathNetwork net = PathNetwork.Instance;
        List<int> paths = new List<int>();
        (_, int nearestPath) = net.NearestPointOnPaths(transform.position, isFuture);
        paths.Add(nearestPath);

        if (Vector2.Distance(net.GetPathPositionA(nearestPath), transform.position) < intersectionSize)
        {
            // near enough to node B to switch paths
            paths.AddRange(net.GetPathConnectionsA(nearestPath, isFuture));
        }

        if (Vector2.Distance(net.GetPathPositionB(nearestPath), transform.position) < intersectionSize)
        {
            // near enough to node A to switch paths
            paths.AddRange(net.GetPathConnectionsB(nearestPath, isFuture));
        }

        return paths;
    }
}