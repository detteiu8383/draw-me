using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawBezierLine : MonoBehaviour
{
    public LineRenderer bezierRenderer;
    public Bezier bezier = new Bezier();

    private DrawPointsLine drawPointsLine;

    // Start is called before the first frame update
    void Awake()
    {
        if (!bezierRenderer)
        {
            bezierRenderer = GetComponent<LineRenderer>();
        }

        drawPointsLine = gameObject.AddComponent<DrawPointsLine>();
        drawPointsLine.lineRenderer = bezierRenderer;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void DrawBezier(int divisions)
    {
        drawPointsLine.points = bezier.GetAllPoints(divisions);
        drawPointsLine.DrawLine();
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < bezier.ControlPoints.Count; i++)
    //    {
    //        Gizmos.color = new Color(1f, 0, 0, 1f);
    //        Gizmos.DrawSphere(bezier.ControlPoints[i], 0.25f);
    //    }
    //    for (int i = 0; i < bezier.Segments.Count; i++)
    //    {
    //        Gizmos.color = new Color(0, 0, 1f, 1f);
    //        Gizmos.DrawLine(bezier.Segments[i].start, bezier.Segments[i].control1);
    //        Gizmos.DrawLine(bezier.Segments[i].end, bezier.Segments[i].control2);
    //    }
    //}

    public void ResetLine()
    {
        bezier = new Bezier();
        drawPointsLine.ResetLine();
    }
}
