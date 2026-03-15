using UnityEngine;

public class TriggerCellSelectionState : MainGameSubState
{
    private CircleType? triggerType;
    private TriggerKind triggerKind;
    private GameManager gm;

    private bool triggerCreated;

    public TriggerCellSelectionState(MainGameState state, TriggerKind kind, CircleType? type, GameManager gm) : base(state)
    {
        triggerKind = kind;
        triggerType = type;
        this.gm = gm;
    }

    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        triggerCreated = false;

        GameServices.Ui.anyCoordinatButton.onClick.AddListener(OnAnyCell);
        GameLog.Event("Subscribed anyCoordinatButton.onClick");
    }

    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");

        GameServices.Ui.anyCoordinatButton.onClick.RemoveListener(OnAnyCell);
        GameLog.Event("Unsubscribed anyCoordinatButton.onClick");
    }

    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        CreateTrigger(new Vector2Int(x, y));
    }

    private void OnAnyCell()
    {
        CreateTrigger(null);
    }

    private void CreateTrigger(Vector2Int? cell)
    {
        if (triggerCreated)
            return;

        Command command = gm.lastPendingCommand;

        // проверяем клетку исходной команды
        if (command is PlaceCircleCommand placeCommand && cell != null)
        {
            Vector2Int commandCell = new Vector2Int(placeCommand.X, placeCommand.Y);

            if (commandCell == cell.Value)
            {
                GameServices.Ui.ShowHint("Нельзя создать триггер на клетке команды");
                return;
            }
        }

        triggerCreated = true;

        int player = GameServices.Turn.CurrentPlayer;

        switch (triggerKind)
        {
            case TriggerKind.EnemyPlaceCircle:
                GameServices.Ether.AddEnemyPlaceTrigger(
                    command,
                    triggerType,
                    cell,
                    player
                );
                break;

            case TriggerKind.SelfPlaceCircle:
                GameServices.Ether.AddSelfPlaceTrigger(
                    command,
                    triggerType,
                    cell,
                    player
                );
                break;

            case TriggerKind.enemyActivate:
            GameServices.Ether.AddEnemyActivateTrigger(
                command,
                triggerType,
                cell,
                player
            );
            break;
        }

        gm.lastPendingCommand = null;

        Debug.Log("Триггер создан");

        mainGameState.ReturnToNormal();
    }
}