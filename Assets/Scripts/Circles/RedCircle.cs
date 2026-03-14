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

            if (neighbor != null  && neighbor.CanBePushed)
            {
                int finishX = nearX + dir.x;
                int finishY = nearY + dir.y;

                // Проверяем, свободна ли новая клетка
                if ((gridManager.GetCellObject(finishX, finishY) == null) ||
                     gridManager.IsCellOccupied(finishX, finishY) || 
                     gridManager.HasBarrierAt(finishX, finishY)) continue; // занято, цель не двигается
                
                targets.Add(neighbor);
            }
        }

        if (targets.Count == 0) return false;
        else
        {
            abilitySystem.StartTargetSelection(this, targets);

            return true;
        }
    }

    public override bool ActivateEther()
    {
        List<Vector2Int> targetCells = new List<Vector2Int>(); 
    
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
                int finishX = nearX + dir.x;
                int finishY = nearY + dir.y;
    
                if (!gridManager.HasBarrierAt(finishX, finishY) && 
                    gridManager.GetCellObject(finishX, finishY) != null)
                {
                    if(gridManager.GetCircleAt(nearX, nearY) != null && gridManager.GetCircleAt(nearX, nearY).Type == CircleType.Core)
                    {
                        continue;
                    }
                    else
                    {
                        targetCells.Add(new Vector2Int(nearX, nearY));   
                    }
                }
            }
        }
    
        if (targetCells.Count == 0) return false;
        else
        {
            abilitySystem.StartTargetCellsSelectionEther(this, targetCells); 
            
            return true;
        }
    }

    
    public override void ApplyEffect()
    {
       
    }
}
