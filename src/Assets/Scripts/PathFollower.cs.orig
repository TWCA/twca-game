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
    [Range(0.0f, 1.0f)] public float perpendicularThreshold = 0.1f;

    public event Action DonePathing;

    private float currentSpeed;
    private bool movedLastFrame;
    private Vector2 jumpStart, jumpEnd;
    private bool isJumping;
    private float jumpDistanceTraveled;

    private List<int> plannedPath;
    private Vector2 plannedEndPosition = Vector2.zero;
    private bool isPathfindingToWalk;

    public PathFollower()
    {
        currentSpeed = minSpeed;
    }

    public void FixedUpdate()
    {
        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();

        (Vector2 position, int path) = net.NearestObstacle(transform.position, isFuture);
        if (path != -1 && Vector2.Distance(position, transform.position) < 20.0)
        {
            BarkManager.Instance.OnNearObstacle(gameObject, net.GetPathName(path));
        }

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

    public void SetCurrentSpeed(float value)
    {
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
        isPathfindingToWalk = false;
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
        if (!IsPathfinding())
            return Vector2.zero;

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
    public Vector2 WalkTowards(Vector2 targetDirection)
    {
        if (isJumping) return Vector2.zero;
<<<<<<< HEAD

=======
        
>>>>>>> main
        if (targetDirection == Vector2.zero)
        {
            if (isPathfindingToWalk)
                StopPathfinding();

            return Vector2.zero;
        }

        if (!isPathfindingToWalk)
            StopPathfinding();

        if (IsPathfinding())
            return Vector2.zero;

        Vector2 goalPosition = (Vector2)transform.position + targetDirection.normalized * walkingLookAheadLength;

        PathNetwork net = PathNetwork.Instance;
        bool isFuture = TimeManager.Instance.IsFuture();
        (Vector2 nearestToGoal, _) = net.NearestPointOnPaths(goalPosition, isFuture);

        if (Vector2.Distance(goalPosition, nearestToGoal) > walkingLookAheadLength * (1 - perpendicularThreshold))
            return Vector2.zero;

        PathfindTo(goalPosition);
        isPathfindingToWalk = true;

        return Vector2.zero;
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
                BarkManager.Instance.OnJumped(gameObject);
            }
            else
            {
                // fail to jump
                StopPathfinding();
                BarkManager.Instance.OnJumpedFailed(gameObject);
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
        // stop half finised paths created using WASD control from making us backtrack
        if (isPathfindingToWalk)
            StopPathfinding();

        jumpDistanceTraveled += currentSpeed * Time.deltaTime * 0.75f;
        Vector2 jumpGroundPosition = Vector2.MoveTowards(jumpStart, jumpEnd, jumpDistanceTraveled);

        float jumpGap = Vector2.Distance(jumpStart, jumpEnd);
        if (jumpDistanceTraveled >= jumpGap)
        {
            transform.position = jumpEnd + (jumpEnd - jumpStart).normalized;
            isJumping = false;
            return;
        }

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