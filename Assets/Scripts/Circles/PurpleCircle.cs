using System.Collections.Generic;
using UnityEngine;

public class PurpleCircle : Circle
{
    public override CircleType Type => CircleType.Purple;

    public override bool Activate()
    {
        List<Circle> targets = new List<Circle>();
        
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
            
            if (neighbor != null && neighbor.Type != CircleType.Core)
            {
                targets.Add(neighbor);
            }
        }

        if (targets.Count == 0) return false;
        
        abilitySystem.StartRemoveChainSelection(this, targets);
        return true;
    }

    public override bool ActivateEther()
    {
        // Для эфира собираем клетки с кругами
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

            Circle neighbor = gridManager.GetCircleAt(nearX, nearY);
            
            if (neighbor != null && neighbor.Type == CircleType.Core) continue;
            
            targetCells.Add(new Vector2Int(nearX, nearY));
        }

        if (targetCells.Count == 0) return false;
        
        abilitySystem.StartRemoveChainEtherSelection(this, targetCells);
        return true;
    }

    public override void ApplyEffect()
    {
    }
}