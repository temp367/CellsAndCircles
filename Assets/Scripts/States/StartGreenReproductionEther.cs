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
        Debug.Log("StartGreenReproductionEther: выбор клетки для размножения");
        grid.HighlightCells(possibleCells, Color.yellow); 
        ui.ShowHint("Выберите клетку для записи деления в эфир");
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
            Command command = new ReproduceCommand(x, y, CircleType.Green, turn.CurrentPlayer, grid, activatingCircle);
            
            cmds.AddCommandToHistory(command, activatingCircle.Type, false, true);

            GameServices.Ability.NotifyCommandCreated(command);
        }
        else
        {
            ui.ShowHint("Нельзя выбрать");
        }
    }
}