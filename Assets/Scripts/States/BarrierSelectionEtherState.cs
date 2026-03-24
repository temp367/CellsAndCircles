using System.Collections.Generic;
using UnityEngine;

public class BarrierSelectionEtherState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possibleCells;
    
    public BarrierSelectionEtherState(MainGameState state, Circle activator, List<Vector2Int> cells) : base(state)
    {
        activatingCircle = activator;
        possibleCells = cells;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        GameServices.Highlight.ShowCells(possibleCells, Color.yellow); 
        GameServices.Ui.ShowHint("Выберите клетку для записи барьера в эфир");
    }
    
    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");

        Vector2Int clickedPos = new Vector2Int(x, y);
        
        if (possibleCells.Contains(clickedPos))
        {
            Command command = new PlaceBarrierCommand(x, y,GameServices.Turn.CurrentPlayer, true);
            GameLog.Ether($"Create ether BarrierCommand from ({activatingCircle.GridX},{activatingCircle.GridY}) -> ({x},{y})");
            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, false, true);

            GameServices.Ability.NotifyEtherCommandCreated(command);
        }
        else
        {
            GameLog.Error($"Invalid target ({x},{y}) for {GetType().Name}");

            GameServices.Ui.ShowHint("Нельзя выбрать");
        }
    }
}