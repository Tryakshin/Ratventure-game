using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    // �������� ���������� ������ �� �������
    public float followSpeed = 2f;

    // ������������ �������� ������ ������������ ������
    public float yOffset = 1f;

    // ������� ������, �� ������� ������� ������
    public Transform target;

    void Update()
    {
        // ���������� ����� ������� ��� ������
        Vector3 newPosition = new Vector3(target.position.x, target.position.y + yOffset, -10f);

        // ������������� Slerp ��� �������� �������� � ����� �������
        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }
}
