using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private int positionCount;
    private Camera mainCamera;
    private Vector3 prePos;
    private Vector3 curPos;
    private bool onDrawArea;
    private Slider inkAmountSlider;

    public GameObject inkAmountBar;
    public float maxInkAmount;
    public float currentInkAmount;
    public float drawTreshold;
    public List<Vector3> posiotionList;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        positionCount = 0;
        mainCamera = Camera.main;
        currentInkAmount = maxInkAmount;

        inkAmountSlider = inkAmountBar.GetComponent<Slider>();
        inkAmountSlider.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // このラインオブジェクトを、位置はカメラ前方10m、回転はカメラと同じになるようキープさせる
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * 10;
        transform.rotation = mainCamera.transform.rotation;

        if (Input.GetMouseButton(0) && currentInkAmount > 0 && onDrawArea)
        {
            // 座標指定の設定をローカル座標系にしたため、与える座標にも手を加える
            curPos = Input.mousePosition;
            curPos.z = 10.0f;

            // マウススクリーン座標をワールド座標に直す
            curPos = mainCamera.ScreenToWorldPoint(curPos);
            // さらにそれをローカル座標に直す。
            curPos = transform.InverseTransformPoint(curPos);

            if (prePos == Vector3.zero)
            {
                prePos = curPos;
            }

            // 前回タッチした座標との距離を求める
            Vector3 dir = curPos - prePos;
            float dist = dir.magnitude;

            // 前回タッチした座標との距離がしきい値を超えた場合は新たに座標をラインレンダラーに追加する
            if (dist > drawTreshold)
            {
                if (currentInkAmount - dist >= 0)
                {
                    currentInkAmount -= dist;
                }
                else
                {
                    dir = dir.normalized;
                    curPos = prePos + currentInkAmount * dir;
                    currentInkAmount = 0;
                }

                posiotionList.Add(curPos);
                positionCount++;
                lineRenderer.positionCount = positionCount;
                lineRenderer.SetPosition(positionCount - 1, curPos);
                prePos = curPos;
                inkAmountSlider.value = currentInkAmount / maxInkAmount;
            }
        }
    }

    public void OnResetButtonClick()
    {
        Reset();
    }

    public void PointerEnter()
    {
        onDrawArea = true;
    }

    public void PointerExit()
    {
        onDrawArea = false;
    }

    private void Reset()
    {
        positionCount = 0;
        posiotionList = new List<Vector3>();
        lineRenderer.positionCount = 0;
        prePos = Vector3.zero;
        currentInkAmount = maxInkAmount;
    }
}
