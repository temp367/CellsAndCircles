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
        GameLog.Action($"ENTER {GetType().Name}");
    }

     public override void Exit()
    {
        GameLog.Action($"EXIT {GetType().Name}");
    }
    
    public override void HandleCellClick(int x, int y)
    {
        GameLog.Action($"Player {GameServices.Turn.CurrentPlayer} click ({x},{y}) in {GetType().Name}");
        
        if(GameServices.Grid.IsCellOccupied(x,y) && GameServices.Grid.GetCircleAt(x,y).Type == CircleType.Core)
        {
            return;
        }
        
        // Создаём команду для установки круга
        Command command = new PlaceCircleCommand(x, y, typeToPlace, GameServices.Turn.CurrentPlayer, true); 
        GameLog.Ether($"Create ether PlaceCommand ({x},{y})");  
        GameServices.CommandSys.AddCommandToHistory(command, typeToPlace, false, true);
        
        GameServices.Ability.NotifyEtherCommandCreated(command);
    }
    
}