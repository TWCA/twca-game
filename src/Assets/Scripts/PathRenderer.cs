using System;
using UnityEngine;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour
{
    [Header("Appearance")] [SerializeField]
    private GameObject pathSegment;

    [Header("Rendering Order")] [SerializeField]
    private int zDepth = 1;

    private List<GameObject> pathSegments = new List<GameObject>();
    private bool initalized = false;

    public static PathRenderer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (!initalized)
        {
            // Awake, OnEnable, Reset, and Start.
            // Four different freaking setup functions, but not ONE of them can guarantee that TimeManager is initialized
            // They all run, one after another, for each object.
            // What's the point?
            // What no job site, green coat, zero-trades, nitwit made this game engine?
            // It's 2am if you can't tell. :3
            TimeManager.Instance.onTimeChanged += DrawTraversablePaths;
            initalized = true;
        }
    }

    /**
     * Draw all traversable paths from the path network. Clears any previously existing paths.
     */
    public void DrawTraversablePaths()
    {
        ClearPaths();

        int count = PathNetwork.Instance.GetPathCount();
        bool isFuture = TimeManager.Instance.IsFuture();

        for (int i = 0; i < count; i++)
        {
            if (PathNetwork.Instance.IsPathTraversableAtTime(i, isFuture))
            {
                Vector2 pointA = PathNetwork.Instance.GetPathPositionA(i);
                Vector2 pointB = PathNetwork.Instance.GetPathPositionB(i);

                DrawPath(pointA, pointB);
            }
        }
    }

    /**
     * Draw a single path as a GameObject with a LineRenderer.
     */
    private void DrawPath(Vector2 pointA, Vector2 pointB)
    {
        // instantiate as a child
        pathSegment = Instantiate(pathSegment, transform);
        pathSegments.Add(pathSegment);

        LineRenderer line = pathSegment.GetComponent<LineRenderer>();
        line.SetPositions(new Vector3[]
        {
            new Vector3(pointA.x, pointA.y, zDepth),
            new Vector3(pointB.x, pointB.y, zDepth)
        });
    }

    /**
     * Destory all the path game objects.
     */
    public void ClearPaths()
    {
        foreach (GameObject segment in pathSegments)
        {
            Destroy(segment);
        }

        pathSegments.Clear();
    }
}