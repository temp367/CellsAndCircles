// Примерная структура новых состояний
using UnityEngine;

public class ActivateCircleEtherState : MainGameSubState
{
    public ActivateCircleEtherState(MainGameState state) : base(state)
    {
    }
    
    public override void Enter()
    {
        Debug.Log($"ActivateCircleEtherState: вошли в состояние Enter()");
    }

    public override void Exit()
    {
        Debug.Log($"ActivateCircleEtherState: вышли из состояния Exit()");
    }
    
    public override void HandleCellClick(int x, int y)
    {
        Circle circleOnCell = grid.GetCircleAt(x, y);

        if (circleOnCell != null)
        {
            if (turn.IsOwnedByCurrentPlayer(circleOnCell) && circleOnCell.CanActivate)
            {
                bool hasTargets = circleOnCell.ActivateEther();
            
                if (!hasTargets)
                {
                    ui.ShowHint("Нет целей для активации");
                }
            }
            else
            {
                ui.ShowHint("Этот круг нельзя активировать");
            }
        }
        else
        {
            ui.ShowHint("Это пустая клетка");
        }
    }
}