using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    // Скорость следования камеры за игроком
    public float followSpeed = 2f;

    // Вертикальное смещение камеры относительно игрока
    public float yOffset = 1f;

    // Целевой объект, за которым следует камера
    public Transform target;

    void Update()
    {
        // Вычисление новой позиции для камеры
        Vector3 newPosition = new Vector3(target.position.x, target.position.y + yOffset, -10f);

        // Использование Slerp для плавного перехода к новой позиции
        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }
}
