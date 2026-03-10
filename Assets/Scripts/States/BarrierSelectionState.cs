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
        Debug.Log("BarrierSelectionState: вошли в режим выбора клетки для барьера");
        
        // Подсвечиваем доступные клетки
        grid.HighlightCells(possiblePositions, Color.cyan);
        
        ui.ShowHint("Выберите клетку для барьера");
    }
    
    public override void Exit()
    {
        Debug.Log("BarrierSelectionState: выходим из режима выбора барьера");
        grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        Vector2Int clickedPos = new Vector2Int(x, y);
        
        if (possiblePositions.Contains(clickedPos))
        {
            Command command = new PlaceBarrierCommand(x, y, turn.CurrentPlayer, grid);
            CommandSystem cmdSystem = GameObject.FindAnyObjectByType<CommandSystem>();
            cmdSystem.ExecuteCommand(command);

            mainGameState.ReturnToNormalAndSwitchPlayer();
        }
        else if(clickedPos.x == activatingCircle.GridX && clickedPos.y == activatingCircle.GridY)
        {
            ui.ShowHint($"Ход игрока {turn.CurrentPlayer}");
            mainGameState.ReturnToNormal();
        }
        else
        {
            ui.ShowHint("Нельзя поставить барьер сюда");
        }
    }
}