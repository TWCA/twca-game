using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    /** Pixels per second. */
    public float speed = 40f;
    
    [Range(5.0f, 100f)] public float walkingLookAheadLength = 25f;

    public event Action DonePathing;

    private List<int> plannedPath;
    private Vector2 plannedEndPosition = Vector2.zero;

    public void FixedUpdate()
    {
        if (!IsPathfinding()) return;

        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();

        // check if the last section of the path is intact
        if (!net.AreNodesConnected(plannedPath[^2], plannedPath[^1], isFuture))
            StopPathfinding(); // last section of path broken, the goal (along this path) is now unreachable

        // check if the last section of the path is intact
        if (!net.AreNodesConnected(plannedPath[^2], plannedPath[^1], isFuture))
            StopPathfinding(); // last section of path broken, the goal (along this path) is now unreachable

        // check path is still valid
        if (!AStarPathfinder.CheckPathStillValid(plannedPath, isFuture))
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
        bool isFuture = TimeManager.Instance.IsFuture();
        (plannedPath, _, plannedEndPosition) =
            AStarPathfinder.CalculatePathBetweenPositions(transform.position, target, isFuture);
        return plannedPath != null;
    }

    /**
     * Stop trying to pathfind to a given position.
     */
    public void StopPathfinding()
    {
        if (plannedPath != null)
        {
            DonePathing?.Invoke();
        }

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
    public Vector2 WalkTowards(Vector2 targetDirection, float delta)
    {
        StopPathfinding();

        Vector2 goalPosition = GetWalkingGoal(targetDirection);
        float stepSize = speed * delta;

        Vector2 pointTowardsGoal = GetNextPointTowardsGoal(goalPosition, stepSize);

        //Debug.DrawLine(transform.position, goalPosition, Color.green);

        transform.position = Vector2.MoveTowards(transform.position, pointTowardsGoal, stepSize);

        return (pointTowardsGoal - (Vector2)transform.position).normalized;
    }

    public Vector2 GetNextPointTowardsGoal(Vector2 goalPosition, float stepSize)
    {
        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();

        (List<int> walkPath, _, Vector2 walkEndPosition) =
            AStarPathfinder.CalculatePathBetweenPositions(transform.position, goalPosition, isFuture);

        if (Vector2.Distance(transform.position, net.GetNodePosition(walkPath[1])) < stepSize)
            walkPath.RemoveAt(0);

        if (walkPath.Count > 2)
            return net.GetNodePosition(walkPath[1]);
        else
            return walkEndPosition;
    }

    public Vector2 GetWalkingGoal(Vector2 targetDirection)
    {
        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();
        
        (_, int nearestPath) = net.NearestPointOnPaths(transform.position, isFuture);
        (Vector2 start, Vector2 end) = net.PathPointsGoingDirection(nearestPath, targetDirection);
        Vector2 pathDirection = (end-start).normalized;

        float alignment = Vector2.Dot(pathDirection, targetDirection);
        
        // if we are trying to walk perpendicular to the path, don't move
        if (alignment < 0.1) 
            return transform.position;

        float distanceFromStartOfPath = Vector2.Distance(transform.position, start);
        
        if (distanceFromStartOfPath > walkingLookAheadLength)
        {
            // try to turn when not satisfied if we haven't turned in a while
            if (alignment < 0.9)
                targetDirection -= pathDirection * (alignment * 0.5f);
        }
        else
        {
            // try to stay straight when not satisfied if we just turned
            if (alignment < 0.9)
                targetDirection += pathDirection * (alignment * 0.5f);
        }
        
        Vector2 goalPosition = (Vector2)transform.position + targetDirection.normalized * walkingLookAheadLength;

        return goalPosition;
    }
}