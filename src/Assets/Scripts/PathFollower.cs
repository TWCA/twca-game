using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    // TODO: replace when we have a time-travel setup!
    private const bool IsFuture = false;

    /** Pixels per second. */
    public float speed = 40f;

    /** The distance in pixels to another path where you may switch. */
    public float intersectionSize = 12f;

    /** The max angle in degrees between target walk direction and the path/ */
    [Range(0.0f, 90.0f)] public float maxPathError = 60f;

    [Range(0.0f, 1.0f)] public float pathLerpRate = 0.9f;

    private List<int> plannedPath;
    private Vector2 plannedEndPosition = Vector2.zero;

    public void FixedUpdate()
    {
        if (!IsPathfinding()) return;

        PathNetwork net = PathNetwork.Instance;

        // check if the last section of the path is intact
        if (!net.AreNodesConnected(plannedPath[^2], plannedPath[^1], IsFuture))
            StopPathfinding(); // last section of path broken, the goal (along this path) is now unreachable

        // check if the last section of the path is intact
        if (!net.AreNodesConnected(plannedPath[^2], plannedPath[^1], IsFuture))
            StopPathfinding(); // last section of path broken, the goal (along this path) is now unreachable

        // check path is still valid
        if (!AStarPathfinder.CheckPathStillValid(plannedPath, IsFuture))
            // if invalid try to recalculate path
            if (!PathfindTo(plannedEndPosition))
                // if failed stop pathfinding
                StopPathfinding();
    }

    public void Update()
    {
        if (!IsPathfinding()) return;

        Vector2 targetPosition = GetPathfindingTarget();
        float stepSize = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, stepSize);

        if (transform.position.Equals(targetPosition))
        {
            plannedPath.RemoveAt(0);

            // if there are no more edges to follow, stop pathing
            if (plannedPath.Count <= 1)
                StopPathfinding();
        }
    }

    /**
     * Begins pathfinding to a particular point, updating its progress automatically.
     * Returns if pathfinding was successful.
     */
    public bool PathfindTo(Vector2 target)
    {
        (plannedPath, _, plannedEndPosition) =
            AStarPathfinder.CalculatePathBetweenPositions(transform.position, target, IsFuture);
        return plannedPath != null;
    }

    /**
     * Stop trying to pathfind to a given position.
     */
    public void StopPathfinding()
    {
        plannedPath = null;
        plannedEndPosition = Vector2.zero;
    }

    /**
     * Check if we are trying to pathfind to a given position.
     */
    public bool IsPathfinding()
    {
        return plannedPath != null;
    }

    public Vector2 GetPathfindingDirection()
    {
        Vector2 targetPosition = GetPathfindingTarget();
        return (targetPosition - (Vector2)transform.position).normalized;
    }

    public Vector2 GetPathfindingTarget()
    {
        if (plannedPath.Count > 2)
            return PathNetwork.Instance.GetNodePosition(plannedPath[1]);
        else
            return plannedEndPosition;
    }

    /**
     * Moves this entity along a direction (or as close as possible) in the path network.
     * This will cancel any attempt to pathfind.
     */
    public Vector2 WalkTowards(Vector2 inputDirection, float delta)
    {
        PathNetwork net = PathNetwork.Instance;

        StopPathfinding();

        Vector2 nearestPoint;
        Vector2 movementDirection = Vector2.zero;
        if (!inputDirection.Equals(Vector2.zero))
        {
            (int path, Vector2 pathEnd) = ChoosePathToWalkOn(inputDirection, IsFuture);
            transform.position = Vector2.MoveTowards(transform.position, pathEnd, speed * delta);
            movementDirection = (pathEnd - (Vector2)transform.position).normalized;

            nearestPoint = net.NearestPointOnPath(path, transform.position, false);
        }
        else
        {
            (nearestPoint, _) = net.NearestPointOnPaths(transform.position, IsFuture);
        }

        float lerpAmount = 1 - Mathf.Pow(1 - pathLerpRate, delta);
        transform.position = Vector2.Lerp(transform.position, nearestPoint, lerpAmount);

        return movementDirection;
    }

    /**
     * Chooses the path to walk on with the most similar direction.
     */
    private (int Path, Vector2 pathEnd) ChoosePathToWalkOn(Vector2 direction, bool isFuture)
    {
        List<int> paths = GetTraversablePaths(isFuture);

        int bestPath = paths[0];
        float bestError = Single.PositiveInfinity;
        Vector2 bestPathEnd = transform.position;

        if (paths.Count == 1)
        {
            bestPath = paths[0];
            (bestError, bestPathEnd) = PathErrorDirectional(paths[0], direction);
        }
        else
        {
            foreach (int path in paths)
            {
                (float error, Vector2 pathEnd) =
                    PathErrorPositional(path, direction);

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

    /**
     * Find how closely this path matches the targetDirection, assuming we can go either way.
     */
    private (float AngleError, Vector2 pathEnd) PathErrorDirectional(int path, Vector2 targetDirection)
    {
        PathNetwork net = PathNetwork.Instance;

        (Vector2 start, Vector2 end) = net.PathPointsGoingDirection(path, targetDirection);
        float angleError = Vector2.Angle(end - start, targetDirection);
        return (angleError, end);
    }

    /**
     * Find how closely this path matches the targetDirection, assuming we must walk one way from our current position.
     */
    private (float AngleError, Vector2 pathEnd) PathErrorPositional(int path, Vector2 targetDirection)
    {
        PathNetwork net = PathNetwork.Instance;

        (Vector2 start, Vector2 end) = net.PathPointsComingFrom(path, transform.position);
        float angleError = Vector2.Angle(end - start, targetDirection);

        return (angleError, end);
    }

    /**
     * Get all paths that can be walked on from a current position and time.
     */
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