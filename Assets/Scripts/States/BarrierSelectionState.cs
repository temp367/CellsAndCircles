using System.Collections.Generic;
using UnityEngine;

// Состояние выбора клетки для барьера (синий круг)
public class BarrierSelectionState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possiblePositions;
    
    public BarrierSelectionState(MainGameState state, Circle activator, List<Vector2Int> positions) : base(state)
    {
        activatingCircle = activator;
        possiblePositions = positions;
    }
    
    public override void Enter()
    {
        GameLog.Action($"ENTER {GetType().Name}");
        
        // Подсвечиваем доступные клетки
        GameServices.Highlight.ShowCells(possiblePositions, Color.cyan);
        
        GameServices.Ui.ShowHint("Выберите клетку для барьера");
    }
    
    public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
        GameServices.Highlight.Clear();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        Vector2Int clickedPos = new Vector2Int(x, y);
        
        if (possiblePositions.Contains(clickedPos))
        {
            Command command = new PlaceBarrierCommand(x, y, GameServices.Turn.CurrentPlayer, false);

            bool success = command.Execute();

            GameLog.Action(success
                ? $"SUCCESS barrier at ({x},{y})"
                : $"FAILED barrier at ({x},{y})"
            );

            GameServices.CommandSys.AddCommandToHistory(command, activatingCircle.Type, success, false);
            
            GameServices.Ability.NotifyGameCommandExecuted(command);

            mainGameState.ReturnToNormalAndSwitchPlayer();
        }
        else if(clickedPos.x == activatingCircle.GridX && clickedPos.y == activatingCircle.GridY)
        {
            GameLog.Action($"Player cancelled ability {GetType().Name}");

            GameServices.Ui.ShowHint($"Ход игрока {GameServices.Turn.CurrentPlayer}");
            mainGameState.ReturnToNormal();
        }
        else
        {
            GameLog.Error($"Invalid target ({x},{y}) for {GetType().Name}");

            GameServices.Ui.ShowHint("Нельзя поставить барьер сюда");
        }
    }
}