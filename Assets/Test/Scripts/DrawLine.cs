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
        // ���̃��C���I�u�W�F�N�g���A�ʒu�̓J�����O��10m�A��]�̓J�����Ɠ����ɂȂ�悤�L�[�v������
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * 10;
        transform.rotation = mainCamera.transform.rotation;

        if (Input.GetMouseButton(0) && currentInkAmount > 0 && onDrawArea)
        {
            // ���W�w��̐ݒ�����[�J�����W�n�ɂ������߁A�^������W�ɂ����������
            curPos = Input.mousePosition;
            curPos.z = 10.0f;

            // �}�E�X�X�N���[�����W�����[���h���W�ɒ���
            curPos = mainCamera.ScreenToWorldPoint(curPos);
            // ����ɂ�������[�J�����W�ɒ����B
            curPos = transform.InverseTransformPoint(curPos);

            if (prePos == Vector3.zero)
            {
                prePos = curPos;
            }

            // �O��^�b�`�������W�Ƃ̋��������߂�
            Vector3 dir = curPos - prePos;
            float dist = dir.magnitude;

            // �O��^�b�`�������W�Ƃ̋������������l�𒴂����ꍇ�͐V���ɍ��W�����C�������_���[�ɒǉ�����
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
