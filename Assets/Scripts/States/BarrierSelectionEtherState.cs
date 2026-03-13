using System.Collections.Generic;
using UnityEngine;

public class BarrierSelectionEtherState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possibleCells;

    public static event System.Action<Command> OnCommandActivateBlueStatic;
    
    public BarrierSelectionEtherState(MainGameState state, Circle activator, List<Vector2Int> cells) : base(state)
    {
        activatingCircle = activator;
        possibleCells = cells;
    }
    
    public override void Enter()
    {
        Debug.Log("BarrierSelectionEtherState: выбор клетки для барьера");
        grid.HighlightCells(possibleCells, Color.yellow); 
        ui.ShowHint("Выберите клетку для записи барьера в эфир");
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
            Command command = new PlaceBarrierCommand(x, y,turn.CurrentPlayer, grid);
            
            cmds.AddCommandToHistory(command, activatingCircle.Type, false, true);

            OnCommandActivateBlueStatic?.Invoke(command);
        }
        else
        {
            ui.ShowHint("Нельзя выбрать");
        }
    }
}