using System.Collections.Generic;
using UnityEngine;

public class BlueCircle : Circle
{
    public override CircleType Type => CircleType.Blue;
    
    public override bool CanBePushed => true; // синий круг можно толкать (пока да)
    
    public override void ApplyEffect()
    {
    }
    
    public override bool Activate()
    {
        // Собираем пустые соседние клетки
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
        gameManager.StartBarrierSelection(this, emptyNeighbors);
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
    
            GameObject neighbor = gridManager.GetCellObject(nearX, nearY);
    
            if (neighbor != null && !gridManager.HasBarrierAt(nearX, nearY))
            {
                if(gridManager.GetCircleAt(nearX, nearY) != null && gridManager.GetCircleAt(nearX, nearY).Type == CircleType.Core)
                {
                    continue;
                }
                else
                {
                    targetsNeibors.Add(new Vector2Int(nearX, nearY));
                }
            }
        }
    
        if (targetsNeibors.Count == 0) return false;
        else
        {
            gameManager.StartBarrierCellsSelectionEther(this, targetsNeibors); 
            
            return true;
        }
    }
}