using UnityEngine;

public class SelfPlaceCircleTrigger : Trigger
{
    public CircleType? TargetType { get; private set; }    

    public SelfPlaceCircleTrigger(int ownerPlayer, CircleType? type, Vector2Int? cell)
        : base(ownerPlayer, cell)
    {
        TargetType = type;
    }

    protected override bool Check(Command command)
    {
        GameLog.Trigger($"Checking trigger {GetType().Name} against {command.GetType().Name}");

        if (command is not PlaceCircleCommand place)
            return false;

        // Реагирует только на своего игрока
        if (place.OwnerPlayer != OwnerPlayer)
            return false;

        // проверка типа круга
        if (TargetType != null && place.Type != TargetType)
            return false;

        // проверка клетки
        if (TargetCell != null)
        {
            if (place.X != TargetCell.Value.x || place.Y != TargetCell.Value.y)
                return false;
        }

        return true;
    }
}