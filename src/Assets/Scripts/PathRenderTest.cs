using UnityEngine;
using System.Collections.Generic;

public class PathRendererTest : MonoBehaviour
{
    [SerializeField] private PathRenderer pathRenderer;

    void Start()
    {
        // Hard-coded test path
        List<Vector3> testPath = new List<Vector3>()
        {
            new Vector3(-4, 0, 0),
            new Vector3(-1, 1, 0),
            new Vector3(2, 1, 0),
            new Vector3(4, -1, 0)
        };

        pathRenderer.DrawPath(testPath);
    }
}
