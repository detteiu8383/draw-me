using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsToBezierTest : MonoBehaviour
{
    public List<Vector3> points;
    public float maxError;
    public LineRenderer pointRenderer;
    public LineRenderer bezierRenderer;
    public int DIVISION_COUNT = 20;

    private Bezier bezier;
    private BezierLine bezierLine = new BezierLine();

    // Start is called before the first frame update
    void Start()
    {
        bezier = new PointsToBezier().fitCurve(points, maxError);
        bezierLine.SetBezier(bezier);
        bezierLine.lineRenderer = bezierRenderer;
        bezierLine.DrawLine(DIVISION_COUNT);
        DrawPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawPoints()
    {
        pointRenderer.positionCount = points.Count;
        pointRenderer.SetPositions(points.ToArray());
    }
}
