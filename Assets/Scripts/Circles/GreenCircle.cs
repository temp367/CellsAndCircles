using UnityEngine;
using System.Collections.Generic;

public class GreenCircle : Circle
{
    public override CircleType Type => CircleType.Green;
    
    public override bool CanBePushed => true; // зелёный круг можно толкать
    
    public override void ApplyEffect()
    {
    }
    
    public override bool Activate()
    {
        List<Vector2Int> emptyNeighbors = new List<Vector2Int>();
        
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

            // Проверка границ
            if (gridManager.GetCellObject(nearX, nearY) == null) continue;   

            
            // Проверка, свободна ли клетка (нет круга и нет барьера)
            if (!gridManager.IsCellOccupied(nearX, nearY) && !gridManager.HasBarrierAt(nearX, nearY))
            {
                emptyNeighbors.Add(new Vector2Int(nearX, nearY));
            }
            
        }

        if (emptyNeighbors.Count == 0)
        {
            return false;
        }
        
        // Запускаем выбор клетки
        gameManager.StartGreenReproduction(this, emptyNeighbors);
        return true;
    }

    public override bool ActivateEther()
    {
        List<Vector2Int> targetsNeibors = new List<Vector2Int>();
        
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

            // Проверка границ
            if (gridManager.GetCellObject(nearX, nearY) == null) continue;   
            if(gridManager.IsCellOccupied(nearX, nearY) && gridManager.GetCircleAt(nearX, nearY).Type == CircleType.Core) continue;   
            
            if (!gridManager.HasBarrierAt(nearX, nearY))
            {
                targetsNeibors.Add(new Vector2Int(nearX, nearY));
            }
            
        }

        if (targetsNeibors.Count == 0) return false;
        else
        {
            gameManager.StartGreenReproductionEther(this, targetsNeibors);
            
            return true;
        }
    }
    
    // Метод для создания нового зелёного круга на выбранной клетке
    public void ReproduceAt(int targetX, int targetY)
    {
        // Создаём новый зелёный круг через GridManager
        bool success = gridManager.PlaceCircle(targetX, targetY, Player, CircleType.Green);
        
        if (success)
        {
            Debug.Log($"Зелёный круг размножился на клетку ({targetX}, {targetY})");
        }
        else
        {
            Debug.LogError($"Не удалось создать зелёный круг на ({targetX}, {targetY})");
        }
    }
}