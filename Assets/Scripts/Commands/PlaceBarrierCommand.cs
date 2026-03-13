using UnityEngine;

public class PlaceBarrierCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }
    
    private GridManager grid;

    public PlaceBarrierCommand(int x, int y, int ownerPlayer, GridManager grid) : base(ownerPlayer)
    {
        this.X = x;
        this.Y = y;
        this.grid = grid;
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            executed = grid.PlaceBarrier(X, Y, OwnerPlayer, grid.CurrentTurn);
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