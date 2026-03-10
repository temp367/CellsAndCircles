public class PlaceCircleCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    
    private CircleType type;
    private GridManager grid;
    private Circle createdCircle;
    
    public PlaceCircleCommand(int x, int y, CircleType type, int ownerPlayer, GridManager grid) : base(ownerPlayer)
    {
        this.X = x;
        this.Y = y;
        this.type = type;
        this.grid = grid;
    }
    
    public override void Execute()
    {
        if (!executed)
        {
            // PlaceCircle возвращает bool, но нам нужна ссылка на созданный круг
            // Придётся немного изменить PlaceCircle, чтобы он возвращал Circle
            bool sucses = grid.PlaceCircle(X, Y, OwnerPlayer, type);
            if(sucses) createdCircle =  grid.GetCircleAt(X, Y);
            executed = true;
            UnityEngine.Debug.Log($"PlaceCircleCommand: выполнен, создан {type} на ({X},{Y})");
        }
    }
    
    public override void Undo()
    {
        if (executed && createdCircle != null)
        {
            grid.RemoveCircle(createdCircle);
            executed = false;
            UnityEngine.Debug.Log($"PlaceCircleCommand: отменён");
        }
    }
    
    public override string GetDescription()
    {
        return $"Поставить {type} на ({X},{Y})";
    }
}