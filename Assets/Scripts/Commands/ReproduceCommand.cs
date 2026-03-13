using UnityEngine;

public class ReproduceCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }

    private int x;
    private int y;
    private CircleType type;
    private GridManager grid;
    private Circle parentCircle;
    
    public ReproduceCommand(int x, int y, CircleType type, int ownerPlayer, GridManager grid, Circle parent) : base(ownerPlayer)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.grid = grid;
        this.parentCircle = parent;
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            executed = grid.PlaceCircle(x, y, OwnerPlayer, type);
        }
        
        return executed;
    }
    
    public override void Undo()
    {
        
    }
    
    public override string GetDescription()
    {
        return $"Размножить {type} на ({x},{y})";
    }
}