using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierLine : MonoBehaviour
{
    public List<Vector3> controlPoints;
    public LineRenderer lineRenderer;

    public int DIVISION_COUNT = 50;
    private Bezier bezier = new Bezier();

    // Start is called before the first frame update
    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void SetBezier(Bezier bezier)
    {
        this.bezier = bezier;
    }

    public void DrawLine(int divisions)
    {
        List<Vector3> drawPoints = bezier.GetAllPoints(divisions);
        lineRenderer.positionCount = drawPoints.Count;
        lineRenderer.SetPositions(drawPoints.ToArray());
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < bezier.ControlPoints.Count; i++)
        {
            Gizmos.color = new Color(1f, 0, 0, 1f);
            Gizmos.DrawSphere(bezier.ControlPoints[i], 0.25f);
        }
        for (int i = 0; i < bezier.Segments.Count; i++)
        {
            Gizmos.color = new Color(0, 0, 1f, 1f);
            Gizmos.DrawLine(bezier.Segments[i].start, bezier.Segments[i].control1);
            Gizmos.DrawLine(bezier.Segments[i].end, bezier.Segments[i].control2);
        }
    }
}
