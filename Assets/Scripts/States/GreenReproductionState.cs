using System.Collections.Generic;
using UnityEngine;

// Состояние выбора клетки для размножения (зелёный круг)
public class GreenReproductionState : MainGameSubState
{
    private Circle activatingCircle;
    private List<Vector2Int> possiblePositions;
    
    public GreenReproductionState(MainGameState state, Circle activator, List<Vector2Int> positions) : base(state)
    {
        activatingCircle = activator;
        possiblePositions = positions;
    }
    
    public override void Enter()
    {
        // Подсвечиваем доступные клетки
        grid.HighlightCells(possiblePositions, Color.green);
        
        ui.ShowHint("Выберите клетку для размножения");
    }
    
    public override void Exit()
    {
        Debug.Log("GreenReproductionState: выходим из режима размножения");
        grid.ClearHighlights();
    }
    
    public override void HandleCellClick(int x, int y)
    {
        Vector2Int clickedPos = new Vector2Int(x, y);
        
        if (possiblePositions.Contains(clickedPos))
        {
            Command command = new ReproduceCommand(x, y, CircleType.Green, turn.CurrentPlayer, grid, activatingCircle);
            
            cmds.AddCommandToHistory(command, activatingCircle.Type, command.Execute(), false);

            mainGameState.ReturnToNormalAndSwitchPlayer();
        }
        else if(clickedPos.x == activatingCircle.GridX && clickedPos.y == activatingCircle.GridY)
        {
            ui.ShowHint($"Ход игрока {turn.CurrentPlayer}");
            mainGameState.ReturnToNormal();
        }
        else
        {
            ui.ShowHint("Нельзя размножиться сюда");
        }
    }
}