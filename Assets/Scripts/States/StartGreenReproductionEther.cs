using System.Collections.Generic;
using UnityEngine;

public class GreenReproductionEtherState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possibleCells;
    
    public GreenReproductionEtherState(MainGameState state, Circle activator, List<Vector2Int> cells) : base(state)
    {
        activatingCircle = activator;
        possibleCells = cells;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        GameServices.Highlight.ShowCells(possibleCells, Color.yellow); // жёлтый для эфира
        GameServices.Ui.ShowHint("Выберите клетку для записи деления в эфир");
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
            Command command = new ReproduceCommand(x, y, CircleType.Green, GameServices.Turn.CurrentPlayer, GameServices.Grid, activatingCircle, true);
            GameLog.Ether($"Create ether ReproduceCommand from ({activatingCircle.GridX},{activatingCircle.GridY}) -> ({x},{y})");
            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, false, true);

            GameServices.Ability.NotifyEtherCommandCreated(command);
        }
        else
        {
            GameServices.Ui.ShowHint("Нельзя выбрать");
        }
    }
}