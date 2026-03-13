using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionEtherState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possibleCells;

     public static event System.Action<Command> OnCommandActivateRedStatic;
    
    public TargetSelectionEtherState(MainGameState state, Circle activator, List<Vector2Int> cells) : base(state)
    {
        activatingCircle = activator;
        possibleCells = cells;
    }
    
    public override void Enter()
    {
        Debug.Log("TargetCellsEtherState: выбор клетки для перемещения в эфире");
        grid.HighlightCells(possibleCells, Color.yellow); // жёлтый для эфира
        ui.ShowHint("Выберите клетку для записи толчка в эфир");
    }
    
    public override void Exit()
    {
        grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        Vector2Int clickedPos = new Vector2Int(x, y);
        
        if (possibleCells.Contains(clickedPos))
        {
            Command command = new PushTargetCommand(activatingCircle, x, y, grid);
            
            cmds.AddCommandToHistory(command, activatingCircle.Type, false, true);

            OnCommandActivateRedStatic?.Invoke(command);
        }
        else
        {
            ui.ShowHint("Нельзя выбрать");
        }
    }
}