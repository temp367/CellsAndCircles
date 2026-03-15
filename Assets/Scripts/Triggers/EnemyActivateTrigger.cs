using UnityEngine;

public class EnemyActivateTrigger : Trigger
{
    public CircleType? TargetType { get; private set; }
    public Vector2Int? TargetCell { get; private set; }

    public EnemyActivateTrigger(int ownerPlayer, CircleType? type, Vector2Int? cell)
        : base(ownerPlayer)
    {
        TargetType = type;
        TargetCell = cell;
    }

    protected override bool Check(Command command)
    {
        int player;
        CircleType commandType;
        Vector2Int position;

        if (command is PushTargetCommand push)
        {
            player = push.Activator.Player;
            commandType = CircleType.Red;
            position = new Vector2Int(push.Activator.GridX, push.Activator.GridY);
        }
        else if (command is PlaceBarrierCommand barrier)
        {
            player = barrier.OwnerPlayer;
            commandType = CircleType.Blue;
            position = new Vector2Int(barrier.X, barrier.Y);
        }
        else if (command is ReproduceCommand reproduce)
        {
            player = reproduce.OwnerPlayer;
            commandType = CircleType.Green;
            position = new Vector2Int(reproduce.Activator.GridX, reproduce.Activator.GridY);
        }
        else
        {
            return false;
        }

        // только враг
        if (player == OwnerPlayer)
            return false;

        // тип противника не совпал с типом который задал игрок
        if (TargetType != null && commandType != TargetType)
            return false;

        // фильтр клетки
        if (TargetCell != null && TargetCell.Value != position)
            return false;

        return true;
    }
}