using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color pathColor = new Color(0.45f, 0.32f, 0.18f); // brown default
    [SerializeField] private float pathWidth = 0.15f; // world units (~15 px)

    [Header("Rendering Order")]
    [SerializeField] private int sortingOrder = -10;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = pathWidth;
        lineRenderer.endWidth = pathWidth;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = pathColor;
        lineRenderer.endColor = pathColor;

        lineRenderer.sortingOrder = sortingOrder;
        lineRenderer.positionCount = 0;
    }

    public void DrawPath(List<Vector3> points)
    {
        if (points == null || points.Count < 2)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }
}
