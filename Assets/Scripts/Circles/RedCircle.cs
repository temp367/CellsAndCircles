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

                bool cellExists = gridManager.GetCellObject(finishX, finishY) != null;

                // если клетка существует — обычная проверка
                if (cellExists)
                {
                    if (gridManager.IsCellOccupied(finishX, finishY) ||
                        gridManager.HasBarrierAt(finishX, finishY))
                        continue;
                }
                else
                {
                    // клетки нет (за границей)
                    // разрешаем только для Core
                    if (neighbor.Type != CircleType.Core)
                        continue;
                }

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
                
                bool cellExists = gridManager.GetCellObject(finishX, finishY) != null;

                // если клетка существует — обычная проверка
                if (cellExists)
                {
                    if (gridManager.HasBarrierAt(finishX, finishY))
                        continue;
                }
                else
                {
                    // клетки нет (за границей)
                    // разрешаем только для Core
                    if (GameServices.Grid.GetCircleAt(nearX, nearY) != null && GameServices.Grid.GetCircleAt(nearX, nearY).Type != CircleType.Core)
                        continue;
                }

                targetCells.Add(new Vector2Int(nearX, nearY));   
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
