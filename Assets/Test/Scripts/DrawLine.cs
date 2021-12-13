using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private int positionCount;
    private Camera mainCamera;
    private Vector3 prePos;
    private Vector3 curPos;

    public float drawTreshold;
    public List<Vector3> posiotionList;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        positionCount = 0;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // ���̃��C���I�u�W�F�N�g���A�ʒu�̓J�����O��10m�A��]�̓J�����Ɠ����ɂȂ�悤�L�[�v������
        transform.position = mainCamera.transform.position + mainCamera.transform.forward * 10;
        transform.rotation = mainCamera.transform.rotation;

        if (Input.GetMouseButton(0))
        {
            // ���W�w��̐ݒ�����[�J�����W�n�ɂ������߁A�^������W�ɂ����������
            curPos = Input.mousePosition;
            curPos.z = 10.0f;

            // �}�E�X�X�N���[�����W�����[���h���W�ɒ���
            curPos = mainCamera.ScreenToWorldPoint(curPos);
            // ����ɂ�������[�J�����W�ɒ����B
            curPos = transform.InverseTransformPoint(curPos);

            // �O��^�b�`�������W�Ƃ̋��������߂�
            Vector3 dir = prePos - curPos;
            float dist = dir.magnitude;

            // �O��^�b�`�������W�Ƃ̋������������l�𒴂����ꍇ�͐V���ɍ��W�����C�������_���[�ɒǉ�����
            if (dist > drawTreshold)
            {
                posiotionList.Add(curPos);
                // ����ꂽ���[�J�����W�����C�������_���[�ɒǉ�����
                positionCount++;
                lineRenderer.positionCount = positionCount;
                lineRenderer.SetPosition(positionCount - 1, curPos);
                prePos = curPos;
            }
        }
        //���Z�b�g����
        if (!(Input.GetMouseButton(0)))
        {
            positionCount = 0;
            posiotionList = new List<Vector3>();
        }
    }
}
