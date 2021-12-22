using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [HideInInspector]
    public Bezier railBezier;

    [SerializeField] Vector3 mapCenter;
    [SerializeField] float mapScale;

    [SerializeField] GameObject bezierLine;
    private DrawBezierLine drawBezierLine;

    [SerializeField] int railDivision;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeMap()
    {
        if (!bezierLine)
        {
            bezierLine = GameObject.Find("bezierLine");
        }
        drawBezierLine = bezierLine.GetComponent<DrawBezierLine>();

        List<Vector3> normalizedControlPoints = railBezier.ControlPoints;

        List<Vector3> scaledControlPoints = new List<Vector3>();

        for (int i = 0; i < normalizedControlPoints.Count; i++)
        {
            Vector3 tmp = normalizedControlPoints[i];
            tmp += mapCenter;
            tmp *= mapScale;
            scaledControlPoints.Add(tmp);
        }

        railBezier = new Bezier
        {
            ControlPoints = scaledControlPoints
        };
        drawBezierLine.bezier = railBezier;
        drawBezierLine.DrawBezier(railDivision);
    }

    private void Reset()
    {
        bezierLine = GameObject.Find("BezierLine");
        railDivision = 20;
        mapScale = 1;
    }
}
