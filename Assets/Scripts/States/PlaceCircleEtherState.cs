// Примерная структура новых состояний
using UnityEngine;

public class PlaceCircleEtherState : MainGameSubState
{
    private CircleType typeToPlace;
    
    public PlaceCircleEtherState(MainGameState state, CircleType type) : base(state)
    {
        typeToPlace = type;
    }
    
    public override void Enter()
    {
        Debug.Log($"PlaceCircleEtherState: вошли в состояние Enter()");
    }

     public override void Exit()
    {
        Debug.Log($"PlaceCircleEtherState: вышли из состояния Exit()");
    }
    
    public override void HandleCellClick(int x, int y)
    {
        if(grid.IsCellOccupied(x,y) && grid.GetCircleAt(x,y).Type == CircleType.Core)
        {
            return;
        }
        
        // Создаём команду для установки круга
        Command command = new PlaceCircleCommand(x, y, typeToPlace, turn.CurrentPlayer, grid);   
        cmds.AddCommandToHistory(command, typeToPlace);

        
        // Возвращаемся в обычный режим и переключаем ход
        //mainGameState.ReturnToNormalAndSwitchPlayer();
    }
    
}