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
        Debug.Log("TargetSelectionState: выбор клетки для перемещения");
        
        // Подсвечиваем цели
        List<Vector2Int> targetPositions = new List<Vector2Int>();
        foreach (Circle target in possibleTargets)
        {
            targetPositions.Add(new Vector2Int(target.GridX, target.GridY));
            Debug.Log($"{target.GridX} {target.GridY}");
        }
        
        grid.HighlightCells(targetPositions, Color.red);
        
        ui.ShowHint("Выберите цель для толчка");
    }
    
    public override void Exit()
    {
        grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        Circle clickedCircle = grid.GetCircleAt(x, y);

        if (possibleTargets.Contains(clickedCircle) && clickedCircle.CanBePushed)
        {
            Command command = new PushTargetCommand(activatingCircle, x, y, grid);
            
            bool sucses = command.Execute();
            cmds.AddCommandToHistory(command, activatingCircle.Type, sucses, false);

            mainGameState.ReturnToNormalAndSwitchPlayer();
        }
        else if(clickedCircle.GridX == activatingCircle.GridX && clickedCircle.GridY == activatingCircle.GridY)
        {
            ui.ShowHint($"Ход игрока {turn.CurrentPlayer}");
            mainGameState.ReturnToNormal();
        }
        else
        {
            ui.ShowHint("Это не цель");
        }
    }
}