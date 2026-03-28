using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    /** Measured in px/s */
    public float minSpeed = 50f;

    /** Measured in px/s */
    public float maxSpeed = 120f;

    /** Measured in px/s */
    public float minJumpSpeed = 110f;

    /** Measured in px/s² */
    public float acceleration = 50f;

    /** Measured in px/s² */
    public float deceleration = 500f;

    /** Measured in px/s² */
    public float gravity = 0.14f;

    /** How far ahead of the player do we pathfind to when walking with WASD */
    [Range(5.0f, 100f)] public float walkingLookAheadLength = 25f;

    /** How much the WASD system should help us to turn when it is detected. Between zero and one.*/
    [Range(0.0f, 0.9f)] public float turnAssistStrength = 0.5f;

    /** How close we have to be to turning before the WASD system helps us turn easier. Between zero and one. */
    [Range(0.0f, 0.5f)] public float turnAssistThreshold = 0.1f;

    /** How close we have to be to walking perpendicular to the path with WASD before we just stop moving. Between zero and one. */
    [Range(0.0f, 0.5f)] public float perpendicularMovementThreshold = 0.1f;

    public event Action DonePathing;

    private float currentSpeed;
    private bool movedLastFrame;
    private Vector2 jumpStart, jumpEnd;
    private bool isJumping = false;
    private float jumpDistanceTraveled;

    private List<int> plannedPath;
    private Vector2 plannedEndPosition = Vector2.zero;

    public PathFollower()
    {
        currentSpeed = minSpeed;
    }

    public void FixedUpdate()
    {
        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();

        if (IsPathfinding())
        {
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
    }

    public void Update()
    {
        if (IsPathfinding())
        {
            Vector2 targetPosition = GetPathfindingNextPointTowardsGoal();
            MoveAndHandleJumps(targetPosition);

            if (IsPathfinding() && Vector2.Distance(transform.position, targetPosition) < currentSpeed * Time.deltaTime)
            {
                plannedPath.RemoveAt(0);

                // if there are no more edges to follow, stop pathing
                if (plannedPath.Count <= 1)
                    StopPathfinding();
            }
        }

        if (isJumping)
            MoveDuringJump();

        if (movedLastFrame)
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, minSpeed, deceleration * Time.deltaTime);

        movedLastFrame = false;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void SetCurrentSpeed(float value) {
        currentSpeed = value;
    }

    public bool IsJumping()
    {
        return isJumping;
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
        Vector2 targetPosition = GetPathfindingNextPointTowardsGoal();
        return (targetPosition - (Vector2)transform.position).normalized;
    }

    public Vector2 GetPathfindingNextPointTowardsGoal()
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

        if (goalPosition.Equals(transform.position))
            return Vector2.zero;

        Vector2 targetPosition = GetNextPointTowardsGoal(goalPosition);

        // Draw line from player to goal position
        //Debug.DrawLine(transform.position, targetPosition, Color.green);

        MoveAndHandleJumps(targetPosition);

        return (targetPosition - (Vector2)transform.position).normalized;
    }

    /**
     * Does pathfinding and calculates the next point that needs to be pathfound to.
     * Indented to be used for WASD control.
     */
    public Vector2 GetNextPointTowardsGoal(Vector2 goalPosition)
    {
        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();

        (List<int> walkPath, _, Vector2 walkEndPosition) =
            AStarPathfinder.CalculatePathBetweenPositions(transform.position, goalPosition, isFuture);

        if (walkPath.Count >= 2)
            if (Vector2.Distance(transform.position, net.GetNodePosition(walkPath[1])) < currentSpeed * Time.deltaTime)
                walkPath.RemoveAt(0);

        if (walkPath.Count > 2)
            return net.GetNodePosition(walkPath[1]);
        else
            return walkEndPosition;
    }

    /**
     * Finds an ideal goal location when walking with WASD.
     * This location is a small distance in-front of the player defined by "walkingLookAheadLength".
     * The location is adjusted near turns to make them easier to take.
     */
    public Vector2 GetWalkingGoal(Vector2 targetDirection)
    {
        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();

        (_, int nearestPath) = net.NearestPointOnPaths(transform.position, isFuture);
        (Vector2 start, Vector2 end) = net.PathPointsGoingDirection(nearestPath, targetDirection);
        Vector2 pathDirection = (end - start).normalized;

        // measure how close our targetDirection is to walking down the path
        // 1.0 -> path perfectly matches target
        // 0.0 -> path is perpendicular to target
        float alignment = Vector2.Dot(pathDirection, targetDirection);
        
        float distanceFromStartOfPath = Vector2.Distance(transform.position, start);
        float distanceFromEndOfPath = Vector2.Distance(transform.position, end);

        // if we are trying to walk perpendicular to the path, don't move
        if (alignment < perpendicularMovementThreshold &&
            distanceFromStartOfPath > perpendicularMovementThreshold &&
            distanceFromEndOfPath > perpendicularMovementThreshold)
            return transform.position;

        if (distanceFromStartOfPath > walkingLookAheadLength)
        {
            // try to turn when not satisfied if we haven't turned in a while
            if (alignment < 1.0f - turnAssistThreshold)
                targetDirection -= pathDirection * (alignment * turnAssistStrength);
        }
        else
        {
            // try to keep going if we just turned
            if (alignment < 1.0f - turnAssistThreshold)
                targetDirection += pathDirection * (alignment * turnAssistStrength);
        }

        Vector2 goalPosition = (Vector2)transform.position + targetDirection.normalized * walkingLookAheadLength;

        return goalPosition;
    }

    /**
     * Handles moving the player and detecting when a jump begins
     */
    private void MoveAndHandleJumps(Vector2 targetPosition)
    {
        if (isJumping)
            return;

        Vector2 nextPosition = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();
        (_, int nearestPath) = net.NearestPointOnPaths(nextPosition, isFuture);

        if (net.DoesPathRequireJump(nearestPath))
        {
            if (currentSpeed > minJumpSpeed)
            {
                // jump
                Vector2 direction = targetPosition - (Vector2)transform.position;
                (jumpStart, jumpEnd) = net.PathPointsGoingDirection(nearestPath, direction);
                isJumping = true;
                jumpDistanceTraveled = 0;
            }
            else
            {
                // fail to jump
                StopPathfinding();
                return;
            }
        }

        transform.position = nextPosition;
        movedLastFrame = true;
    }

    /**
     * Handles moving the player over the arc of a jump
     */
    private void MoveDuringJump()
    {
        jumpDistanceTraveled += currentSpeed * Time.deltaTime * 0.75f;
        Vector2 jumpGroundPosition = Vector2.MoveTowards(jumpStart, jumpEnd, jumpDistanceTraveled);

        float jumpGap = Vector2.Distance(jumpStart, jumpEnd);
        if (jumpDistanceTraveled >= jumpGap)
            isJumping = false;

        float halfGap = jumpGap * 0.5f;
        float jumpHeight = Squared(halfGap * gravity) - Squared((jumpDistanceTraveled - halfGap) * gravity);
        transform.position = jumpGroundPosition + Vector2.up * jumpHeight;
        movedLastFrame = true;
    }

    /**
     * Does what it says on the tin. x²
     */
    private float Squared(float x)
    {
        return x * x;
    }
}