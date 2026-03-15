using UnityEngine;

public class EnemyPlaceCircleTrigger : Trigger
{
    public CircleType? TargetType { get; private set; }
    public Vector2Int? TargetCell { get; private set; }

    public EnemyPlaceCircleTrigger(int ownerPlayer, CircleType? type, Vector2Int? cell)
        : base(ownerPlayer)
    {
        TargetType = type;
        TargetCell = cell;
    }

    protected override bool Check(Command command)
    {
        if (command is not PlaceCircleCommand) return false;

        // свой круг не триггерит
        if (((PlaceCircleCommand)command).OwnerPlayer == OwnerPlayer) return false;
            
        // проверка типа
        if (TargetType != null && ((PlaceCircleCommand)command).Type != TargetType)
        {
            Debug.Log($"{TargetType} != {((PlaceCircleCommand)command).Type}");
            return false;
        }

        // проверка клетки
        if (TargetCell != null)
        {
            if (((PlaceCircleCommand)command).X != TargetCell.Value.x || ((PlaceCircleCommand)command).Y != TargetCell.Value.y)
                return false;
        }

        return true;
    }
}