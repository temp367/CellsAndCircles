using UnityEngine;

public class ReproduceCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }

    private CircleType type;
    private GridManager grid;
    public Circle Activator{ get; private set; }
    
    public ReproduceCommand(int x, int y, CircleType type, int ownerPlayer, GridManager grid, Circle parent, bool isEtherCommand) : base(ownerPlayer, isEtherCommand)
    {
        X = x;
        Y = y;
        this.type = type;
        this.grid = grid;
        this.Activator = parent;
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            executed = grid.PlaceCircle(X, Y, OwnerPlayer, type);
        }
        
        return executed;
    }
    
    public override void Undo()
    {
        
    }
    
    public override string GetDescription()
    {
        return $"Размножить {type} на ({X},{Y})";
    }
}