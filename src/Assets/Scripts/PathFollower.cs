using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float speed = 1f;
    public float intersectionSize = 0.4f;
    /* the max angle in degrees between target walk direction and the path */
    public float maxPathError = 45f;

    public void WalkOnPath(Vector2 target)
    {
        // temporary
        bool isFuture = false;

        Vector2 targetDirection = (target - (Vector2)transform.position).normalized;

        PathNetwork net = PathNetwork.Instance;


        Vector2 nearestPoint;
        if (!targetDirection.Equals(Vector2.zero))
        {
            (int path, Vector2 pathEnd) = PlanWalkOnPath(targetDirection, isFuture);
            transform.position = Vector2.MoveTowards(transform.position, pathEnd, speed * Time.deltaTime);
            
            nearestPoint = net.NearestPointOnPath(path, transform.position);
        }
        else
        {
            (nearestPoint, _) = net.NearestPointOnPath(transform.position, isFuture);
        }

        transform.position = Vector2.Lerp(transform.position, nearestPoint, 1 - Mathf.Pow(0.1f, Time.deltaTime));
    }

    private (int Path, Vector2 pathEnd) PlanWalkOnPath(Vector2 targetDirection, bool isFuture)
    {
        (int nearestPath, List<int> otherPaths) = GetTraversablePaths(isFuture);

        // for now let's assume the best path is the closest
        int bestPath = nearestPath;
        (float bestError, Vector2 bestPathEnd) = PathErrorDirectional(nearestPath, targetDirection);

        // then lets see if the other paths better match the target direction
        foreach (int otherPath in otherPaths)
        {
            (float error, Vector2 pathEnd) =
                PathErrorPositional(otherPath, targetDirection);

            if (error < bestError)
            {
                bestPath = otherPath;
                bestError = error;
                bestPathEnd = pathEnd;
            }
        }
        
        // check if we can even walk
        if (bestError > maxPathError) 
            return (nearestPath, transform.position);

        return (bestPath, bestPathEnd);
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

    private (int Nearest, List<int> Others) GetTraversablePaths(bool isFuture)
    {
        PathNetwork net = PathNetwork.Instance;
        (_, int nearestPath) = net.NearestPointOnPath(transform.position, isFuture);

        List<int> otherPaths = new List<int>();
        if (Vector2.Distance(net.GetPathNodeAPosition(nearestPath), transform.position) < intersectionSize)
        {
            // near enough to node B to switch paths
            otherPaths.AddRange(net.GetPathNodeAConnections(nearestPath, isFuture));
        }

        if (Vector2.Distance(net.GetPathNodeBPosition(nearestPath), transform.position) < intersectionSize)
        {
            // near enough to node A to switch paths
            otherPaths.AddRange(net.GetPathNodeBConnections(nearestPath, isFuture));
        }

        return (nearestPath, otherPaths);
    }
}