using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawPointsLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> points;

    // Start is called before the first frame update
    void Awake()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawLine()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public void ResetLine()
    {
        lineRenderer.positionCount = 0;
        points = new List<Vector3>();
    }
}
