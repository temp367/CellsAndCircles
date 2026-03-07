using UnityEngine;

// Действие: поставить круг на указанную клетку
public class PlaceEtherAction : EtherAction
{
    private int targetX;
    private int targetY;
    private CircleType circleType;
    
    public PlaceEtherAction(int ownerPlayer, int x, int y, CircleType type) : base(ownerPlayer)
    {
        targetX = x;
        targetY = y;
        circleType = type;
    }
    
    public override bool CanExecute(GridManager gridManager, TurnManager turnManager)
    {
        // Проверяем, что клетка свободна
        if (gridManager.GetCircleAt(targetX, targetY) != null)
        {
            Debug.Log($"PlaceEtherAction: клетка ({targetX}, {targetY}) занята");
            return false;
        }
        
        // Проверяем, что нет барьера
        if (gridManager.HasBarrierAt(targetX, targetY))
        {
            Debug.Log($"PlaceEtherAction: на клетке ({targetX}, {targetY}) барьер");
            return false;
        }
        
        // Проверяем, что не пытаемся поставить на Core
        Circle circle = gridManager.GetCircleAt(targetX, targetY);
        if (circle != null && circle.Type == CircleType.Core)
        {
            Debug.Log($"PlaceEtherAction: нельзя ставить на Core");
            return false;
        }
        
        return true;
    }
    
    public override void Execute(GridManager gridManager)
    {
        bool success = gridManager.PlaceCircle(targetX, targetY, OwnerPlayer, circleType);
        if (success)
        {
            Debug.Log($"PlaceEtherAction: выполнен, поставлен {circleType} круг на ({targetX}, {targetY})");
        }
        else
        {
            Debug.LogError($"PlaceEtherAction: не удалось выполнить!");
        }
    }
    
    public override string GetDescription()
    {
        return $"Поставить {circleType} круг на ({targetX}, {targetY})";
    }
}