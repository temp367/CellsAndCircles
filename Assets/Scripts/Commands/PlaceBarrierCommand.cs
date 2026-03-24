using UnityEngine;

public class PlaceBarrierCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }
    

    public PlaceBarrierCommand(int x, int y, int ownerPlayer, bool isEtherCommand) : base(ownerPlayer, isEtherCommand)
    {
        this.X = x;
        this.Y = y;
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            executed = GameServices.Grid.PlaceBarrier(X, Y, OwnerPlayer, GameServices.Grid.CurrentTurn);
        }

        return executed;
    }
    
    public override void Undo()
    {
    }
    
    public override string GetDescription()
    {
        return $"Установить барьер на ({X},{Y})";
    }
}