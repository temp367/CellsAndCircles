using UnityEngine;

public class PlaceBarrierCommand : Command
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public CircleType Type { get; private set; }
    
    private GridManager grid;
    private bool placed = false;

    
    public PlaceBarrierCommand(int x, int y, int ownerPlayer, GridManager grid) : base(ownerPlayer)
    {
        this.X = x;
        this.Y = y;
        this.grid = grid;
    }
    
    public override void Execute()
    {
        if (!placed)
        {
            placed = grid.PlaceBarrier(X, Y, OwnerPlayer, grid.CurrentTurn);
            if (placed)
                Debug.Log($"PlaceBarrierCommand: барьер установлен на ({X},{Y})");
        }
    }
    
    public override void Undo()
    {
        if (placed)
        {
            grid.RemoveBarrierAt(X, Y);
            placed = false;
            Debug.Log($"PlaceBarrierCommand: барьер удалён");
        }
    }
    
    public override string GetDescription()
    {
        return $"Установить барьер на ({X},{Y})";
    }
}