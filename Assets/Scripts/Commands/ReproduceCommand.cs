public class ReproduceCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }

    private int x;
    private int y;
    private CircleType type;
    private GridManager grid;
    private Circle createdCircle;
    private Circle parentCircle;
    
    public ReproduceCommand(int x, int y, CircleType type, int ownerPlayer, GridManager grid, Circle parent) : base(ownerPlayer)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.grid = grid;
        this.parentCircle = parent;
    }
    
    public override void Execute()
    {
        if (!executed)
        {
            bool sucses = grid.PlaceCircle(x, y, OwnerPlayer, type);
            if(sucses) createdCircle = grid.GetCircleAt(x, y);

            executed = true;
            UnityEngine.Debug.Log($"ReproduceCommand: создан {type} на ({x},{y})");
        }
    }
    
    public override void Undo()
    {
        if (executed && createdCircle != null)
        {
            grid.RemoveCircle(createdCircle);
            executed = false;
            UnityEngine.Debug.Log($"ReproduceCommand: отменён");
        }
    }
    
    public override string GetDescription()
    {
        return $"Размножить {type} на ({x},{y})";
    }
}