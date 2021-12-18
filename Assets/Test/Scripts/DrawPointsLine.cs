using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawPointsLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> points;

    // Start is called before the first frame update
    void Start()
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

    public void drawLine()
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public void resetLine()
    {
        lineRenderer.positionCount = 0;
        points = new List<Vector3>();
    }
}
