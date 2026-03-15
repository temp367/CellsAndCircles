// Примерная структура новых состояний
using UnityEngine;

public class ActivateCircleEtherState : MainGameSubState
{
    public ActivateCircleEtherState(MainGameState state) : base(state)
    {
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
    }

    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        Circle circleOnCell = GameServices.Grid.GetCircleAt(x, y);

        if (circleOnCell != null)
        {
            if (GameServices.Turn.IsOwnedByCurrentPlayer(circleOnCell) && circleOnCell.CanActivate)
            {
                bool hasTargets = circleOnCell.ActivateEther();
            
                if (!hasTargets)
                {
                    GameServices.Ui.ShowHint("Нет целей для активации");
                }
            }
            else
            {
                GameServices.Ui.ShowHint("Этот круг нельзя активировать");
            }
        }
        else
        {
            GameServices.Ui.ShowHint("Это пустая клетка");
        }
    }
}