using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionEtherState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possibleCells;
    
    public TargetSelectionEtherState(MainGameState state, Circle activator, List<Vector2Int> cells) : base(state)
    {
        activatingCircle = activator;
        possibleCells = cells;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        GameServices.Grid.HighlightCells(possibleCells, Color.yellow); // жёлтый для эфира
        GameServices.Ui.ShowHint("Выберите клетку для записи толчка в эфир");
    }
    
    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
        GameServices.Grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        Vector2Int clickedPos = new Vector2Int(x, y);
        
        if (possibleCells.Contains(clickedPos))
        {
            Command command = new PushTargetCommand(activatingCircle, x, y, true);
            GameLog.Ether($"Create ether PushTargetCommand from ({activatingCircle.GridX},{activatingCircle.GridY}) -> ({x},{y})");
            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, false, true);

            GameServices.Ability.NotifyEtherCommandCreated(command);
        }
        else
        {
            GameServices.Ui.ShowHint("Нельзя выбрать");
        }
    }
}