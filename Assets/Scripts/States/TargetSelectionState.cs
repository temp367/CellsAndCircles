using System.Collections.Generic;
using UnityEngine;

// Состояние выбора цели для красного круга
public class TargetSelectionState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Circle> possibleTargets;
    
    public TargetSelectionState(MainGameState state, Circle activator, List<Circle> targets) : base(state)
    {
        activatingCircle = activator;
        possibleTargets = targets;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        
        // Подсвечиваем цели
        List<Vector2Int> targetPositions = new List<Vector2Int>();
        foreach (Circle target in possibleTargets)
        {
            targetPositions.Add(new Vector2Int(target.GridX, target.GridY));
            Debug.Log($"{target.GridX} {target.GridY}");
        }
        
        GameServices.Grid.HighlightCells(targetPositions, Color.red);
        
        GameServices.Ui.ShowHint("Выберите цель для толчка");
    }
    
    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
        GameServices.Grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        Circle clickedCircle = GameServices.Grid.GetCircleAt(x, y);

        if (possibleTargets.Contains(clickedCircle) && clickedCircle.CanBePushed)
        {
            Command command = new PushTargetCommand(activatingCircle, x, y, false);
            
            bool success = command.Execute();

            GameLog.Action(success
                ? $"SUCCESS target at ({x},{y})"
                : $"FAILED target at ({x},{y})"
            );

            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, success, false);

            GameServices.Ability.NotifyGameCommandExecuted(command);

            mainGameState.ReturnToNormalAndSwitchPlayer();
        }
        else if(clickedCircle.GridX == activatingCircle.GridX && clickedCircle.GridY == activatingCircle.GridY)
        {
            GameLog.Action($"Player cancelled ability {GetType().Name}");

            GameServices.Ui.ShowHint($"Ход игрока {GameServices.Turn.CurrentPlayer}");
            mainGameState.ReturnToNormal();
        }
        else
        {
            GameLog.Error($"Invalid target ({x},{y}) for {GetType().Name}");

            GameServices.Ui.ShowHint("Это не цель");
        }
    }
}