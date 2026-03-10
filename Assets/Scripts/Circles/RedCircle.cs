using System.Collections.Generic;
using UnityEngine;

public class RedCircle : Circle
{
    public override CircleType Type => CircleType.Red;

    public override bool Activate()
    {
        List<Circle> targets = new List<Circle>();
        
        // Все соседи клетки с кругом
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
        };

        foreach (var dir in directions)
        {
            int nearX = GridX + dir.x;
            int nearY = GridY + dir.y;

            Circle neighbor = gridManager.GetCircleAt(nearX, nearY);

            // Круг прнадалежит другому игроку?
            if (neighbor != null  && neighbor.CanBePushed)
            {
                int finishX = nearX + dir.x;
                int finishY = nearY + dir.y;

                // Проверяем, свободна ли новая клетка
                if (gridManager.IsCellOccupied(finishX, finishY) || gridManager.HasBarrierAt(finishX, finishY) || (gridManager.GetCellObject(finishX, finishY) == null)) continue; // занято, цель не двигается

                targets.Add(neighbor);
            }
        }

        if (targets.Count == 0)
        {
            Debug.Log("Нет вражеских кругов рядом, активация бесполезна.");
            return false;
        }
        else
        {
            // Запускаем процесс выбора цели
            gameManager.StartTargetSelection(this, targets);   

            return true;
        }
    }

    public void PushTarget(Circle target)
    {
        if (!target.CanBePushed)
        {
            Debug.Log($"Цель {target.GetType().Name} нельзя толкнуть!");
            return;
        }

        // Определяем направление от себя к цели
        int directX = target.GridX - GridX;
        int directY = target.GridY - GridY;

        // Проверка, что цель соседняя 
        if (Mathf.Abs(directX) > 1 || Mathf.Abs(directY) > 1 || (directX == 0 && directY == 0))
        {
            Debug.LogError("Цель не является соседней!");
            return;
        }

        // Новая позиция для цели
        int newX = target.GridX + directX;
        int newY = target.GridY + directY;

        // Получаем GameObject цели
        GameObject targetObj = target.gameObject;

        // Находим клетку назначения
        GameObject newCell = gridManager.GetCellObject(newX, newY);

        if (newCell == null)
        {
            Debug.LogError($"Клетка назначения ({newX}, {newY}) не найдена!");
            return;
        }

        // Перемещаем объект в новую клетку
        targetObj.transform.SetParent(newCell.transform);
        targetObj.transform.localPosition = Vector3.zero;

        // Обновляем данные в словаре GridManager
        gridManager.MoveCircle(target.GridX, target.GridY, newX, newY, target);
        // Обновляем координаты в самом круге
        target.UpdatePosition(newX, newY);

        target.OnPushedBy(this);

        Debug.Log($"Круг {target.GetType().Name} перемещён с ({target.GridX - directX}, {target.GridY - directY}) на ({newX}, {newY})");
    }

    public override void ApplyEffect()
    {
       
    }
}
