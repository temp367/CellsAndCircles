using System.Collections.Generic;
using UnityEngine;

// Состояние выбора цели для фиолетового круга в эфире
public class RemoveChainEtherSelectionState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possibleCells;
    
    public RemoveChainEtherSelectionState(MainGameState state, Circle activator, List<Vector2Int> cells) : base(state)
    {
        activatingCircle = activator;
        possibleCells = cells;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        
        // Подсвечиваем клетки с возможными целями
        GameServices.Highlight.ShowCells(possibleCells, Color.yellow); // фиолетовый
        
        GameServices.Ui.ShowHint("Выберите круг для удаления цепочки (эфир)");
    }
    
    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        Vector2Int clickedPos = new Vector2Int(x, y);
        Circle clickedCircle = GameServices.Grid.GetCircleAt(x, y);

        if (possibleCells.Contains(clickedPos))
        {
            // Создаём команду для эфира
            Command command = new RemoveChainCommand(activatingCircle, clickedCircle, true);
            GameLog.Ether($"Create ether RemoveChainCommand from ({activatingCircle.GridX},{activatingCircle.GridY}) -> ({x},{y})");
            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, false, true);

            GameServices.Ability.NotifyEtherCommandCreated(command);
        }
        else
        {
            GameLog.Error($"Invalid target ({x},{y}) for {GetType().Name} (ether)");
            GameServices.Ui.ShowHint("Нельзя выбрать");
        }
    }
}