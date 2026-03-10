using UnityEngine;
using UnityEngine.InputSystem; // нужно для новой Input System

public class MouseInputHandler : MonoBehaviour
{
    // Ссылка на камеру (можно найти автоматически)
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    void Update()
    {
        // Была ли нажата левая кнопка мыши в этом кадре?
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Получаем позицию мыши в экранных координатах
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Выполняем Raycast из камеры в точку мыши
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                // Есть компонент CellClick на объекте, в который попали?
                CellClick cellClick = hit.collider.GetComponent<CellClick>();
                if (cellClick != null)
                {
                    // Вызываем событие клика
                    cellClick.HandleClick();
                }

                // Проверяем ZoneCell
                ZoneCell zoneCell = hit.collider.GetComponent<ZoneCell>();
                if (zoneCell != null)
                {
                    zoneCell.HandleClick();
                    return;
                }
            }
        }
    }
}