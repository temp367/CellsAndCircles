public class PlaceCircleCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }
    
    
    public PlaceCircleCommand(int x, int y, CircleType type, int ownerPlayer, bool isEtherCommand) : base(ownerPlayer, isEtherCommand)
    {
        this.X = x;
        this.Y = y;
        Type = type;
    }
    
    public override bool Execute()
    {
        if (!executed)
        {
            GameLog.Command($"Player {OwnerPlayer} PLACE {Type} circle at ({X},{Y})");

            bool sucses = GameServices.Grid.PlaceCircle(X, Y, OwnerPlayer, Type);

            if(sucses) 
            {
                GameLog.Action("Command executed successfully");
            }
            else
            {
                GameLog.Error($"Command FAILED (cell occupied) ({X},{Y})");
            }
            
            executed = true;
        }
        
        return executed;
    }
    
    public override void Undo()
    {
        
    }
    
    public override string GetDescription()
    {
        return $"Поставить {Type} на ({X},{Y})";
    }
}