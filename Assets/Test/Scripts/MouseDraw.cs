using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseDraw : MonoBehaviour
{
    [SerializeField] Button resetButton;
    [SerializeField] Button generateBezierButton;
    [SerializeField] Slider inkLevelSlider;

    [SerializeField] GameObject pointsLine;
    private DrawPointsLine drawPointsLine;

    [SerializeField] GameObject bezierLine;
    private DrawBezierLine drawBezierLine;

    [SerializeField] float maxInkLevel;
    private float currInkLevel;

    [SerializeField] float drawThreshold;
    [SerializeField] float maxError;
    [SerializeField] int divisionCount;

    private List<Vector3> drawPoints = new List<Vector3>();
    private Vector3 currMousePos;
    private Vector3 prevMousePos;

    private bool isMouseOnDrawArea;
    public bool generated;

    private RectTransform drawAreaRectTransform;
    private new BoxCollider2D collider;
    private float factor;


    // Start is called before the first frame update
    void Start()
    {
        drawAreaRectTransform = gameObject.GetComponent<RectTransform>();

        if (!resetButton)
        {
            resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
        }
        resetButton.onClick.AddListener(OnResetButtonClick);


        if (!generateBezierButton)
        {
            generateBezierButton = GameObject.Find("GenerateBezierButton").GetComponent<Button>();
        }
        generateBezierButton.onClick.AddListener(OnGenerateButtonClick);

        if (!inkLevelSlider)
        {
            inkLevelSlider = GameObject.Find("InkLevelSlider").GetComponent<Slider>();
        }

        if (!pointsLine)
        {
            pointsLine = GameObject.Find("PointsLine");
        }
        drawPointsLine = pointsLine.GetComponent<DrawPointsLine>();

        if (!bezierLine)
        {
            bezierLine = GameObject.Find("bezierLine");
        }
        drawBezierLine = bezierLine.GetComponent<DrawBezierLine>();

        collider = gameObject.AddComponent<BoxCollider2D>();

        factor = drawAreaRectTransform.rect.size.x;
        collider.size = drawAreaRectTransform.rect.size;

        ResetDraw();
    }

    // Update is called once per frame
    void Update()
    {
        if (!generated && Input.GetMouseButton(0) && currInkLevel > 0 && isMouseOnDrawArea)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                drawAreaRectTransform,
                Input.mousePosition,
                Camera.main,
                out Vector2 vec2MousePos
            );
            currMousePos = vec2MousePos;
            currMousePos.z = -0.1f;

            //Debug.Log(vec2MousePos);
            //Debug.Log(currMousePos);

            if (prevMousePos == new Vector3(0, 0, -10f))
            {
                prevMousePos = currMousePos;
                return;
            }

            Vector3 dir = currMousePos - prevMousePos;
            float dist = dir.magnitude;

            if (dist > drawThreshold)
            {
                if (currInkLevel - dist >= 0)
                {
                    currInkLevel -= dist;
                }
                else
                {
                    dir = dir.normalized;
                    currMousePos = prevMousePos + currInkLevel * dir;
                    currInkLevel = 0;
                }

                drawPoints.Add(currMousePos);
                drawPointsLine.points = drawPoints;
                drawPointsLine.DrawLine();

                prevMousePos = currMousePos;

                inkLevelSlider.value = currInkLevel / maxInkLevel;
            }
        }
    }

    private void OnResetButtonClick()
    {
        Debug.Log("Reset.");
        ResetDraw();
    }

    private void ResetDraw()
    {
        prevMousePos = new Vector3(0, 0, -10f);
        drawPoints = new List<Vector3>();
        currInkLevel = maxInkLevel;
        drawPointsLine.ResetLine();
        drawBezierLine.ResetLine();
        inkLevelSlider.value = 1;
        pointsLine.SetActive(true);
        generateBezierButton.interactable = true;
        generated = false;
    }

    private void OnGenerateButtonClick()
    {
        Debug.Log("Generate Bezier Curve.");

        Bezier bezier = DrawnBezier;
        drawBezierLine.bezier = bezier;
        drawBezierLine.DrawBezier(divisionCount);
        pointsLine.SetActive(false);
        generateBezierButton.interactable = false;
        generated = true;
    }

    public Bezier DrawnBezier
    {
        get
        {
            return new PointsToBezier().FitCurve(drawPoints, maxError);
        }
    }

    public Bezier NormalizedDrawnBezier
    {
        get
        {
            List<Vector3> normalizedControlpoints = new List<Vector3>();
            List<Vector3> origControlpoints = DrawnBezier.ControlPoints;

            for (int i = 0; i < origControlpoints.Count; i++)
            {
                Vector3 tmp = origControlpoints[i] / factor;
                tmp.z = 0;
                normalizedControlpoints.Add(tmp);
            }

            return new Bezier
            {
                ControlPoints = normalizedControlpoints
            };
        }
    }

    private void OnMouseEnter()
    {
        isMouseOnDrawArea = true;
    }

    private void OnMouseExit()
    {
        isMouseOnDrawArea = false;
    }

    private void Reset()
    {
        resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
        generateBezierButton = GameObject.Find("GenerateBezierButton").GetComponent<Button>();
        inkLevelSlider = GameObject.Find("InkLevelSlider").GetComponent<Slider>();
        pointsLine = GameObject.Find("PointsLine");
        bezierLine = GameObject.Find("BezierLine");

        maxInkLevel = 2000;
        drawThreshold = 5f;
        maxError = 10;
        divisionCount = 10;
    }
}
